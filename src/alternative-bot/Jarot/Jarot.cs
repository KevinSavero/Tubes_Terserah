using System;
using System.Drawing;
using System.Collections.Generic;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Jarot : Bot
{
    private readonly Dictionary<int, ScannedBotEvent> _scannedEnemies = new();

    private int    _sniperTargetId   = -1;
    private double _targetSpeed      = double.MaxValue;
    private double _targetDistance   = 0;

    private int    _strafeDirection  = 1;
    private int    _turnsSinceStrafe = 0;
    private const int StrafePeriod   = 30; 

    static void Main(string[] args) { new Jarot().Start(); }

    Jarot() : base(BotInfo.FromFile("Jarot.json")) { }

    public override void Run()
    {
        BodyColor   = Color.Black;       
        TurretColor = Color.DarkRed;    
        RadarColor  = Color.LimeGreen;  
        BulletColor = Color.Red;          
        ScanColor   = Color.Lime;

        SetTurnRadarRight(double.PositiveInfinity);

        while (IsRunning)
        {
            _turnsSinceStrafe++;

            SelectHighestHitProbabilityTarget();

            if (_sniperTargetId >= 0 && _scannedEnemies.TryGetValue(_sniperTargetId, out var target))
            {
                ExecuteSniperShot(target);
            }

            ExecutePerpendicularMovement();

            Go();
        }
    }

    private void SelectHighestHitProbabilityTarget()
    {
        _sniperTargetId = -1;
        _targetSpeed    = double.MaxValue;

        foreach (var kvp in _scannedEnemies)
        {
            double speed = Math.Abs(kvp.Value.Speed);

            if (speed < _targetSpeed ||
                (Math.Abs(speed - _targetSpeed) < 0.5 &&
                DistanceTo(kvp.Value.X, kvp.Value.Y) < _targetDistance))
            {
                _targetSpeed    = speed;
                _sniperTargetId = kvp.Key;
                _targetDistance = DistanceTo(kvp.Value.X, kvp.Value.Y);
            }
        }
    }

    private void ExecuteSniperShot(ScannedBotEvent target)
    {
        double distance = DistanceTo(target.X, target.Y);

        double bulletPower;
        if (distance > 400)
        {
            bulletPower = 1.0; 
        }
        else if (distance > 200)
        {
            bulletPower = 2.0; 
        }
        else
        {
            bulletPower = 3.0; 
        }

        double bulletSpeed   = 20 - 3 * bulletPower;
        double travelTime    = distance / bulletSpeed; 

        double futureX = target.X + Math.Sin(DegreesToRadians(target.Direction)) * target.Speed * travelTime;
        double futureY = target.Y + Math.Cos(DegreesToRadians(target.Direction)) * target.Speed * travelTime;

        double gunBearingToFuturePos = GunBearingTo(futureX, futureY);
        SetTurnGunLeft(gunBearingToFuturePos);

        if (Math.Abs(gunBearingToFuturePos) < 5)
        {
            SetFire(bulletPower);
        }

        double radarBearing = RadarBearingTo(target.X, target.Y);
        if (Math.Abs(radarBearing) > 5)
        {
            SetTurnRadarLeft(radarBearing * 1.2); 
        }
    }

    private void ExecutePerpendicularMovement()
    {
        if (_turnsSinceStrafe >= StrafePeriod)
        {
            _strafeDirection  *= -1; 
            _turnsSinceStrafe  = 0;
        }

        if (_sniperTargetId >= 0 && _scannedEnemies.TryGetValue(_sniperTargetId, out var target))
        {
            double bearingToTarget = BearingTo(target.X, target.Y);

            double perpendicularBearing = bearingToTarget + (90 * _strafeDirection);
            SetTurnLeft(NormalizeRelativeAngle(perpendicularBearing));
            SetForward(80); 
        }
        else
        {
            SetTurnRight(20);
            SetForward(50);
        }

        AvoidWallsSniper();
    }

    private void AvoidWallsSniper()
    {
        double margin = 100;
        if (X < margin || X > (ArenaWidth - margin) ||
            Y < margin || Y > (ArenaHeight - margin))
        {
            double centerBearing = BearingTo(ArenaWidth / 2.0, ArenaHeight / 2.0);
            SetTurnLeft(centerBearing);
            SetForward(120);
        }
    }

    private static double DegreesToRadians(double degrees)
        => degrees * Math.PI / 180.0;

    public override void OnScannedBot(ScannedBotEvent e)
    {
        _scannedEnemies[e.ScannedBotId] = e;
    }

    public override void OnBotDeath(BotDeathEvent e)
    {
        _scannedEnemies.Remove(e.VictimId);
        if (_sniperTargetId == e.VictimId)
            _sniperTargetId = -1;
    }

    public override void OnHitWall(HitWallEvent e)
    {
        SetBack(40);
        SetTurnRight(90);
        _strafeDirection *= -1; 
    }

    public override void OnHitBot(HitBotEvent e)
    {
        SetBack(60);
        SetFire(2.0);
    }
}
