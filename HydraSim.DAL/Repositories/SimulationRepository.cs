using HydraSim.Domain.Components;
using HydraSim.Domain.Simulation;
using Newtonsoft.Json;

namespace HydraSim.DAL.Repositories
{
    public class SimulationRepository : ISimulationRepository
    {
        private string GetSessionKey(int id) => $"SimulationComponents_{id}";

        public void SaveToSession(HydraulicSimulation simulation, int id, Action<string, string> sessionSetter)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var json = JsonConvert.SerializeObject(simulation.Components, settings);
            sessionSetter(GetSessionKey(id), json);
        }

        public HydraulicSimulation? LoadFromSession(int id, Func<string, string?> sessionGetter)
        {
            var json = sessionGetter(GetSessionKey(id));
            if (string.IsNullOrEmpty(json)) return null;

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var components = JsonConvert.DeserializeObject<List<HydraulicComponent>>(json, settings);

            var simulation = new HydraulicSimulation();
            foreach (var component in components)
                simulation.AddComponent(component);

            RebuildConnections(simulation, id);
            return simulation;
        }

        public HydraulicSimulation BuildSimulation(int id)
        {
            var simulation = new HydraulicSimulation();

            if (id == 0)
            {
                simulation.AddComponent(new Pump { CX = 1, CY = 1 });
                simulation.AddComponent(new Pipe { CX = 1, CY = 2 });
                simulation.AddComponent(new PressureGauge { CX = 1, CY = 3 });
                simulation.AddComponent(new Pipe { CX = 1, CY = 4 });
                simulation.AddComponent(new Resistance { CX = 1, CY = 5, PressureDrop = 300 });
                simulation.AddComponent(new Pipe { CX = 1, CY = 6 });
                simulation.AddComponent(new PressureGauge { CX = 1, CY = 7 });
                simulation.AddComponent(new Pipe { CX = 2, CY = 7, Rotation = 90 });
                simulation.AddComponent(new Resistance { CX = 3, CY = 7, PressureDrop = 200 });
                simulation.AddComponent(new Pipe { CX = 4, CY = 7, Rotation = 90 });
                simulation.AddComponent(new PressureGauge { CX = 5, CY = 7 });
                simulation.AddComponent(new Pipe { CX = 5, CY = 6 });
                simulation.AddComponent(new Resistance { CX = 5, CY = 5, PressureDrop = 100 });
                simulation.AddComponent(new Pipe { CX = 5, CY = 4 });
                simulation.AddComponent(new PressureGauge { CX = 5, CY = 3 });
                simulation.AddComponent(new Pipe { CX = 5, CY = 2 });
                simulation.AddComponent(new Tank { CX = 5, CY = 1 });
                simulation.AddComponent(new Pipe { CX = 4, CY = 1, Rotation = 90 });
                simulation.AddComponent(new Pipe { CX = 3, CY = 1, Rotation = 90 });
                simulation.AddComponent(new Pipe { CX = 2, CY = 1, Rotation = 90 });
            }
            else if (id == 1)
            {
                simulation.AddComponent(new Pump { CX = 1, CY = 1 });
                simulation.AddComponent(new Pipe { CX = 1, CY = 2 });
                simulation.AddComponent(new PressureGauge { CX = 1, CY = 3 });
                simulation.AddComponent(new Pipe { CX = 1, CY = 4 });
                simulation.AddComponent(new Pipe { CX = 2, CY = 3, Rotation = 90 });
                simulation.AddComponent(new Motor { CX = 3, CY = 3, RequiredPressure = 200 });
                simulation.AddComponent(new Pipe { CX = 3, CY = 4 });
                simulation.AddComponent(new PressureGauge { CX = 3, CY = 5 });
                simulation.AddComponent(new Pipe { CX = 3, CY = 6 });
                simulation.AddComponent(new ReliefValve { CX = 1, CY = 5, MaxPressure = 300 });
                simulation.AddComponent(new Pipe { CX = 1, CY = 6 });
                simulation.AddComponent(new PressureGauge { CX = 1, CY = 7 });
                simulation.AddComponent(new Pipe { CX = 2, CY = 7, Rotation = 90 });
                simulation.AddComponent(new Tank { CX = 3, CY = 7 });
                simulation.AddComponent(new Pipe { CX = 4, CY = 7, Rotation = 90 });
                simulation.AddComponent(new Pipe { CX = 5, CY = 7, isCorner = true });
                simulation.AddComponent(new Pipe { CX = 5, CY = 6 });
                simulation.AddComponent(new Pipe { CX = 5, CY = 5 });
                simulation.AddComponent(new Pipe { CX = 5, CY = 4 });
                simulation.AddComponent(new Pipe { CX = 5, CY = 3 });
                simulation.AddComponent(new Pipe { CX = 5, CY = 2 });
                simulation.AddComponent(new Pipe { CX = 5, CY = 1, isCorner = true, Rotation = 90 });
                simulation.AddComponent(new Pipe { CX = 4, CY = 1, Rotation = 90 });
                simulation.AddComponent(new Pipe { CX = 3, CY = 1, Rotation = 90 });
                simulation.AddComponent(new Pipe { CX = 2, CY = 1, Rotation = 90 });
            }

            RebuildConnections(simulation, id);
            return simulation;
        }

        public void RebuildConnections(HydraulicSimulation simulation, int id)
        {
            var c = simulation.Components;
            foreach (var comp in c) comp.Outputs.Clear();

            if (id == 0)
            {
                c[0].Outputs.Add(c[1]);
                c[1].Outputs.Add(c[2]);
                c[2].Outputs.Add(c[3]);
                c[3].Outputs.Add(c[4]);
                c[4].Outputs.Add(c[5]);
                c[5].Outputs.Add(c[6]);
                c[6].Outputs.Add(c[7]);
                c[7].Outputs.Add(c[8]);
                c[8].Outputs.Add(c[9]);
                c[9].Outputs.Add(c[10]);
                c[10].Outputs.Add(c[11]);
                c[11].Outputs.Add(c[12]);
                c[12].Outputs.Add(c[13]);
                c[13].Outputs.Add(c[14]);
                c[14].Outputs.Add(c[15]);
                c[15].Outputs.Add(c[16]);
                c[16].Outputs.Add(c[17]);
                c[17].Outputs.Add(c[18]);
                c[18].Outputs.Add(c[19]);
            }
            else if (id == 1)
            {
                var pump = c[0]; var pipe1 = c[1]; var gauge1 = c[2];
                var pipe2 = c[3]; var pipeRV = c[4];
                var motor = c[5] as Motor;
                var pipe3 = c[6]; var gauge2 = c[7]; var pipe4 = c[8];
                var rv = c[9] as ReliefValve;
                var pipe5 = c[10]; var gauge3 = c[11]; var pipe6 = c[12];
                var tank = c[13];
                var pipe7 = c[14]; var pipe8 = c[15]; var pipe9 = c[16];
                var pipe10 = c[17]; var pipe11 = c[18]; var pipe12 = c[19];
                var pipe13 = c[20]; var pipe14 = c[21]; var pipe15 = c[22];
                var pipe16 = c[23]; var pipe17 = c[24];

                pump.Outputs.Add(pipe1);
                pipe1.Outputs.Add(gauge1);
                gauge1.Outputs.Add(pipe2);

                if (rv.IsOpen)
                {
                    pipe2.Outputs.Add(rv);
                    rv.Outputs.Add(pipe5);
                    pipe5.Outputs.Add(gauge3);
                    gauge3.Outputs.Add(pipe6);
                    pipe6.Outputs.Add(tank);
                }
                else
                {
                    pipe2.Outputs.Add(pipeRV);
                    pipeRV.Outputs.Add(motor);
                    motor.Outputs.Add(pipe3);
                    pipe3.Outputs.Add(gauge2);
                    gauge2.Outputs.Add(pipe4);
                    pipe4.Outputs.Add(tank);
                }

                tank.Outputs.Add(pipe7);
                pipe7.Outputs.Add(pipe8);
                pipe8.Outputs.Add(pipe9);
                pipe9.Outputs.Add(pipe10);
                pipe10.Outputs.Add(pipe11);
                pipe11.Outputs.Add(pipe12);
                pipe12.Outputs.Add(pipe13);
                pipe13.Outputs.Add(pipe14);
                pipe14.Outputs.Add(pipe15);
                pipe15.Outputs.Add(pipe16);
                pipe16.Outputs.Add(pipe17);
            }
        }
    }
}
