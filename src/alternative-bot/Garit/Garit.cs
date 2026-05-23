using System;
using System.Drawing;
using System.Collections.Generic;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Garit : Bot
{
    private double _moveDirection = 1;
    
    private readonly Dictionary<int, ScannedBotEvent> _detectedEnemies = new();
    private readonly Dictionary<int, double> _enemyEnergyHistory = new();
    private readonly Random _rand = new();

    private int _targetBotId = -1;

    static void Main(string[] args) { new Garit().Start(); }

    Garit() : base(BotInfo.FromFile("Garit.json")) { }

    public override void Run()
    {
        BodyColor   = Color.Black;
        TurretColor = Color.FromArgb(20, 20, 20);
        RadarColor  = Color.FromArgb(40, 40, 40);
        BulletColor = Color.White;
        ScanColor   = Color.FromArgb(30, 30, 30);

        SetTurnRadarRight(double.PositiveInfinity);

        while (IsRunning)
        {
            SelectWeakestTarget();

            SetForward(100 * _moveDirection);

            if (_targetBotId == -1)
            {
                if (RadarTurnRemaining == 0)
                {
                    SetTurnRadarRight(double.PositiveInfinity);
                }
                SetTurnRight(15);
            }

            Go();
        }
    }

    private void SelectWeakestTarget()
    {
        if (_detectedEnemies.Count == 0) { _targetBotId = -1; return; }
        
        int bestId = -1;
        double lowestHP = double.MaxValue;

        foreach (var kvp in _detectedEnemies)
        {
            if (kvp.Value.Energy < lowestHP)
            {
                lowestHP = kvp.Value.Energy;
                bestId = kvp.Key;
            }
        }
        _targetBotId = bestId;
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        _detectedEnemies[e.ScannedBotId] = e;

        double distance = DistanceTo(e.X, e.Y);
        double bearing = BearingTo(e.X, e.Y);

        if (!_enemyEnergyHistory.TryGetValue(e.ScannedBotId, out double previousEnergy))
        {
            previousEnergy = 100.0;
        }

        double energyDrop = previousEnergy - e.Energy;
        if (energyDrop > 0 && energyDrop <= 3.0)
        {
            _moveDirection *= -1;
            double dodgeAngle = 85 + _rand.Next(15); 
            SetTurnRight(dodgeAngle - bearing);
            SetForward((130 + _rand.Next(50)) * _moveDirection);
        }
        else
        {
            if (e.ScannedBotId == _targetBotId)
            {
                SetTurnRight(90 - bearing);
                SetForward(110 * _moveDirection);
            }
        }

        _enemyEnergyHistory[e.ScannedBotId] = e.Energy;

        if (e.ScannedBotId == _targetBotId)
        {
            double radarBearing = RadarBearingTo(e.X, e.Y);
            SetTurnRadarLeft(radarBearing * 1.2);

            double firePower = (distance < 200) ? 3.0 : (distance < 500 ? 2.0 : 1.0);
            if (Energy < 20) firePower = 0.5;

            double bulletSpeed = 20 - 3 * firePower;
            double travelTime = distance / bulletSpeed;

            double futureX = e.X + Math.Sin(e.Direction * Math.PI / 180.0) * e.Speed * travelTime;
            double futureY = e.Y + Math.Cos(e.Direction * Math.PI / 180.0) * e.Speed * travelTime;

            double gunBearingToFuture = GunBearingTo(futureX, futureY);
            SetTurnGunLeft(gunBearingToFuture);

            if (Math.Abs(gunBearingToFuture) < 8 && GunHeat == 0 && Energy > firePower)
            {
                SetFire(firePower);
            }
        }

        if (distance < 130 && e.ScannedBotId == _targetBotId)
        {
            _moveDirection = -1;
            SetBack(140);
        }
    }

    public override void OnBotDeath(BotDeathEvent e)
    {
        _detectedEnemies.Remove(e.VictimId);
        _enemyEnergyHistory.Remove(e.VictimId);
        if (_targetBotId == e.VictimId) _targetBotId = -1;
    }

    public override void OnHitWall(HitWallEvent e)
    {
        Stop(); 
        double centerBearing = BearingTo(ArenaWidth / 2.0, ArenaHeight / 2.0);
        Forward(-60 * _moveDirection); 
        TurnLeft(centerBearing);
        _moveDirection *= -1; 
        Forward(120 * _moveDirection);
    }

    public override void OnHitBot(HitBotEvent e)
    {
        SetFire(3.0);
        _moveDirection *= -1;
        SetForward(120 * _moveDirection);
    }
}