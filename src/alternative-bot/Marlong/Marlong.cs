using System;
using System.Drawing;
using System.Collections.Generic;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Marlong : Bot
{
    private readonly Dictionary<int, ScannedBotEvent> _scannedEnemies = new();

    private int _currentTargetId = -1;

    private int _turnCount = 0;

    static void Main(string[] args) { new Marlong().Start(); }

    Marlong() : base(BotInfo.FromFile("Marlong.json")) { }

    public override void Run()
    {
        BodyColor   = Color.MidnightBlue;       
        TurretColor = Color.Black;    
        RadarColor  = Color.DarkSlateGray;  
        BulletColor = Color.Gold;          
        ScanColor   = Color.Cyan;

        SetTurnRadarRight(double.PositiveInfinity);

        while (IsRunning)
        {
            _turnCount++;

            SelectGreedyTarget();

            if (_currentTargetId >= 0 && _scannedEnemies.TryGetValue(_currentTargetId, out var target))
            {
                ExecuteGreedyAttack(target);
            }

            ExecuteSurvivalMovement();

            Go();
        }
    }

    private void SelectGreedyTarget()
    {
        if (_scannedEnemies.Count == 0)
        {
            _currentTargetId = -1;
            return;
        }

        int    bestId        = -1;
        double lowestEnergy  = double.MaxValue;

        foreach (var kvp in _scannedEnemies)
        {
            if (kvp.Value.Energy < lowestEnergy)
            {
                lowestEnergy = kvp.Value.Energy;
                bestId       = kvp.Key;
            }
        }

        _currentTargetId = bestId;
    }

    private void ExecuteGreedyAttack(ScannedBotEvent target)
    {
        double bearingToTarget = BearingTo(target.X, target.Y);
        double gunTurnNeeded   = NormalizeRelativeAngle(GunDirection - Direction + bearingToTarget);

        SetTurnGunLeft(gunTurnNeeded);

        double bulletPower;
        if (target.Energy <= 4.0)
        {
            bulletPower = 1.0;
        }
        else if (target.Energy <= 16.0)
        {
            bulletPower = 2.0;
        }
        else
        {
            bulletPower = 3.0;
        }

        if (Math.Abs(GunBearingTo(target.X, target.Y)) < 10)
        {
            SetFire(bulletPower);
        }
    }
    
    private void ExecuteSurvivalMovement()
    {
        if (_scannedEnemies.Count == 0)
        {
            SetTurnRight(10);
            SetForward(50);
            return;
        }

        double avgX = 0, avgY = 0;
        foreach (var kvp in _scannedEnemies)
        {
            avgX += kvp.Value.X;
            avgY += kvp.Value.Y;
        }
        avgX /= _scannedEnemies.Count;
        avgY /= _scannedEnemies.Count;
        
        double bearingToCrowd   = BearingTo(avgX, avgY);
        double escapeDirection  = NormalizeRelativeAngle(bearingToCrowd + 180); 

        SetTurnLeft(escapeDirection);
        SetForward(100);

        AvoidWalls();
    }

    private void AvoidWalls()
    {
        double margin = 80; 
        double x = X, y = Y;

        bool nearWall = x < margin || x > (ArenaWidth - margin) ||
                        y < margin || y > (ArenaHeight - margin);

        if (nearWall)
        {
            double centerBearing = BearingTo(ArenaWidth / 2.0, ArenaHeight / 2.0);
            SetTurnLeft(centerBearing);
            SetForward(150);
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        _scannedEnemies[e.ScannedBotId] = e;
    }

    public override void OnBotDeath(BotDeathEvent e)
    {
        _scannedEnemies.Remove(e.VictimId);

        if (_currentTargetId == e.VictimId)
        {
            _currentTargetId = -1;
        }
    }

    public override void OnHitWall(HitWallEvent e)
    {
        SetBack(50);
        SetTurnRight(90);
    }

    public override void OnHitBot(HitBotEvent e)
    {
        if (e.IsRammed) 
        {
            SetFire(3.0); 
    }
}
}
