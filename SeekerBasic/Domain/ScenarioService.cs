using Lendsum.Crosscutting.Common;
using Lendsum.Infrastructure.Core;
using SeekerBasic.Domain.Dtos;
using SeekerBasic.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeekerBasic.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class ScenarioService : IScenarioService
    {
        private IUnitOfWork work;
        private Guid[] scenarioNames = {
            new Guid("00000000000000000000000000000001"),
            new Guid("00000000000000000000000000000002"),
            new Guid("00000000000000000000000000000003")};

        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioService"/> class.
        /// </summary>
        /// <param name="work">The work.</param>
        public ScenarioService(IUnitOfWork work)
        {
            this.work = work;
        }

        /// <summary>
        /// Gets the sceneario.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ScenarioAggregate GetSceneario(int id)
        {
            using (var context = new ThreadContextContainer())
            {
                InitIfNeeded();
                var aggregate = this.work.GetAggregate<ScenarioAggregate>(scenarioNames[id]);

                this.work.Commit();
                return aggregate;
            }
        }

        public ScenarioAggregate Move(int id, DirectionEnum direction)
        {
            using (var context = new ThreadContextContainer())
            {
                var aggregate = this.work.GetAggregate<ScenarioAggregate>(scenarioNames[id]);
                this.work.Handle(new PlayerMoved() { AggregateUid = scenarioNames[id], Direction = direction });

                this.work.Commit();
                return aggregate;
            }
        }

        public ScenarioAggregate Reset(int id)
        {
            using (var context = new ThreadContextContainer())
            {
                var aggregate = this.work.GetAggregate<ScenarioAggregate>(scenarioNames[id]);
                this.work.Handle(new ScenarioReset() { AggregateUid = scenarioNames[id] });
                this.work.Commit();

                return aggregate;
            }
        }

        private void InitIfNeeded()
        {

            var scene1 = this.work.GetAggregate<ScenarioAggregate>(scenarioNames[1]);
            var scene2 = this.work.GetAggregate<ScenarioAggregate>(scenarioNames[2]);

            DoScene0();
            DoScene1();
            //DoScene2();
        }

        private void DoScene2()
        {
            throw new NotImplementedException();
        }

        private void DoScene1()
        {
            var scene0 = this.work.GetAggregate<ScenarioAggregate>(scenarioNames[1]);
            if (scene0 != null) return;
            List<ScenarioItem> items = new List<ScenarioItem>();
            var id = scenarioNames[1];

            MakeWalls(items);

            items.Add(new ScenarioItem() { X = 25, Y = 25, ItemType = ScenarioItemType.Player });
            items.Add(new ScenarioItem() { X = 30, Y = 3, ItemType = ScenarioItemType.Target });

            items.AddRange(MakeWall(28, 2, 6, DirectionEnum.Down));
            items.AddRange(MakeWall(29,8, 6, DirectionEnum.Right));

            this.work.HandleAndCreateIfNeeded<AddedItem, ScenarioAggregate>(new AddedItem() { AggregateUid = id, Items = items });
        }

        private void DoScene0()
        {
            var scene0 = this.work.GetAggregate<ScenarioAggregate>(scenarioNames[0]);
            if (scene0 != null) return;
            List<ScenarioItem> items = new List<ScenarioItem>();
            var id = scenarioNames[0];

            MakeWalls(items);

            items.Add(new ScenarioItem() { X = 25, Y = 25, ItemType = ScenarioItemType.Player });
            items.Add(new ScenarioItem() { X = 30, Y = 2, ItemType = ScenarioItemType.Target });

            this.work.HandleAndCreateIfNeeded<AddedItem, ScenarioAggregate>(new AddedItem() { AggregateUid = id, Items = items });
        }

        private static void MakeWalls(List<ScenarioItem> items)
        {
            items.AddRange(MakeWall(1, 1, 50, DirectionEnum.Right));
            items.AddRange(MakeWall(50, 2, 49, DirectionEnum.Down));
            items.AddRange(MakeWall(49, 50, 49, DirectionEnum.Left));
            items.AddRange(MakeWall(1,49, 48, DirectionEnum.Up));
        }

        private static IEnumerable<ScenarioItem> MakeWall(int x, int y, int lenght, DirectionEnum direction, ScenarioItemType itemType=ScenarioItemType.Block)
        {
            List<ScenarioItem> items = new List<ScenarioItem>();
            var block = new ScenarioItem() { X = x, Y = y, ItemType = itemType };
            items.Add(block);
            for (int i=1;i<lenght;i++)
            {
                block=block.MoveTo(direction);
                items.Add(block);
            }

            return items;
        }
    }
}


