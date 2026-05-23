    using System;
    using System.Drawing;
    using System.Collections.Generic;
    using Robocode.TankRoyale.BotApi;
    using Robocode.TankRoyale.BotApi.Events;

    public class Bokir : Bot
    {
        private readonly Dictionary<int, ScannedBotEvent> _scannedEnemies = new();

        private int    _closestEnemyId  = -1;
        private double _closestDistance = double.MaxValue;

        private bool _inRamMode = false;

        static void Main(string[] args) { new Bokir().Start(); }

        Bokir() : base(BotInfo.FromFile("Bokir.json")) { }

        public override void Run()
        {
            BodyColor   = Color.FromArgb(128, 0, 0);      
            TurretColor = Color.FromArgb(40, 40, 40);    
            RadarColor  = Color.Crimson;                  
            BulletColor = Color.Red;                     
            ScanColor   = Color.FromArgb(255, 69, 0);    

            SetTurnRadarRight(double.PositiveInfinity);

            while (IsRunning)
            {
                SelectClosestEnemy();

                if (_closestEnemyId >= 0 && _scannedEnemies.TryGetValue(_closestEnemyId, out var target))
                {
                    ChaseAndRam(target);
                }
                else
                {
                    SetTurnRight(15);
                    SetForward(50);
                }

                Go();
            }
        }

        private void SelectClosestEnemy()
        {
            _closestEnemyId  = -1;
            _closestDistance = double.MaxValue;

            foreach (var kvp in _scannedEnemies)
            {
                double dist = DistanceTo(kvp.Value.X, kvp.Value.Y);

                if (dist < _closestDistance)
                {
                    _closestDistance = dist;
                    _closestEnemyId  = kvp.Key;
                }
            }

            _inRamMode = _closestDistance < 50;
        }

        private void ChaseAndRam(ScannedBotEvent target)
        {
            double bearingToEnemy = BearingTo(target.X, target.Y);

            SetTurnLeft(bearingToEnemy);
            SetForward(double.PositiveInfinity); 

            double gunBearing = GunBearingTo(target.X, target.Y);
            SetTurnGunLeft(gunBearing);

            if (_inRamMode && Math.Abs(gunBearing) < 15)
            {
                SetFire(3.0); 
            }
            else if (_closestDistance < 150 && Math.Abs(gunBearing) < 20)
            {
                SetFire(2.0);
            }

            double radarBearing = RadarBearingTo(target.X, target.Y);
            SetTurnRadarLeft(radarBearing);
        }

        public override void OnScannedBot(ScannedBotEvent e)
        {
            _scannedEnemies[e.ScannedBotId] = e;
        }

        public override void OnBotDeath(BotDeathEvent e)
        {
            _scannedEnemies.Remove(e.VictimId);

            if (_closestEnemyId == e.VictimId)
            {
                _closestEnemyId  = -1;
                _closestDistance = double.MaxValue;
            }
        }

        public override void OnHitWall(HitWallEvent e)
        {
            SetBack(30);
            SetTurnRight(45);
        }

        public override void OnHitBot(HitBotEvent e)
        {
            SetFire(3.0);
            SetForward(50);
        }
    }
