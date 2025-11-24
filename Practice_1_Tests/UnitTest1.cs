using Practice_1_;

namespace Practice_1_Tests
{
    public class MapCellTests
    {
        [Test]
        public void MapCellCreate_ConstructWithDriver_PropertiesCorrect()
        {
            var cell = new MapCell(1,1,true);

            Assert.AreEqual(1, cell.x);
            Assert.AreEqual(1, cell.y);
            Assert.IsTrue(cell.isDriver);
            Assert.IsFalse(cell.isOrder);
        }
        [Test]
        public void MapCellCreate_ConstructWithoutDriver_PropertiesCprrect()
        {
            var cell = new MapCell(1, 1);

            Assert.AreEqual(1, cell.x);
            Assert.AreEqual(1, cell.y);
            Assert.IsFalse(cell.isDriver);
            Assert.IsFalse(cell.isOrder);
        }
        [Test]
        public void MapCellCreate_ConstructWithNegativeX_ExceptionsCorrect()
        {
            Assert.Throws<ArgumentException>(()=> new MapCell(-1, 1));
        }
        [Test]
        public void MapCellCreate_ConstructWithNegativeY_ExceptionsCorrect()
        {
            Assert.Throws<ArgumentException>(() => new MapCell(1, -1));
        }
    }

    public class GridMapTests
    {

        [Test]
        public void GridMap_Constructor_ValidParameters_CreatesInstance()
        {
            var gridMap = new GridMap(10, 10, 5);

            Assert.IsNotNull(gridMap);
        }

        [Test]
        public void GridMap_Constructor_InvalidN_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new GridMap(0, 10, 5));
        }

        [Test]
        public void GridMap_Constructor_InvalidM_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new GridMap(10, -1, 5));
        }

        [Test]
        public void GridMap_Constructor_NegativeDrivers_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new GridMap(10, 10, -1));
        }

        [Test]
        public void GenerateMap_ValidParameters_ReturnsMapCells()
        {
            var gridMap = new GridMap(5, 5, 5);

            var result = gridMap.GenerateMap();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.GetLength(0));
            Assert.AreEqual(5, result.GetLength(1));
        }

        [Test]
        public void GenerateMap_TooSmallMap_ThrowsException()
        {
            var gridMap = new GridMap(2, 2, 10);

            var ex = Assert.Throws<Exception>(() => gridMap.GenerateMap());
            Assert.That(ex.Message, Does.Contain("Слишком маленькая карта"));
        }

        [Test]
        public void GenerateMap_TooFewDrivers_ThrowsException()
        {
            var gridMap = new GridMap(5, 5, 3);

            var ex = Assert.Throws<Exception>(() => gridMap.GenerateMap());
            Assert.That(ex.Message, Does.Contain("Должно быть указано минимум 5 водителей"));
        }

        [Test]
        public void GenerateMap_CreatesCorrectNumberOfDrivers()
        {
            var gridMap = new GridMap(10, 10, 7);

            var map = gridMap.GenerateMap();

            int driverCount = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (map[i, j].isDriver)
                        driverCount++;
                }
            }
            Assert.AreEqual(7, driverCount);
        }

        [Test]
        public void GenerateMap_CreatesExactlyOneOrder()
        {
            var gridMap = new GridMap(10, 10, 5);

            var map = gridMap.GenerateMap();

            int orderCount = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (map[i, j].isOrder) orderCount++;
                }
            }
            Assert.AreEqual(1, orderCount);
        }

        [Test]
        public void GenerateMap_OrderNotOnDriverPosition()
        {
            var gridMap = new GridMap(10, 10, 5);

            var map = gridMap.GenerateMap();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var cell = map[i, j];
                    if (cell.isOrder)
                    {
                        Assert.IsFalse(cell.isDriver, "Заказ не должен находиться на позиции водителя");
                    }
                }
            }
        }

        [Test]
        public void EnumerationSearch_FindsNearestDrivers()
        {
            var gridMap = new GridMap(5, 5, 5);
            gridMap.GenerateMap();

            TestDelegate testDelegate = () => gridMap.EnumerationSearch();

            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void RadialSearch_FindsNearestDrivers()
        {
            var gridMap = new GridMap(5, 5, 5);
            gridMap.GenerateMap();

            TestDelegate testDelegate = () => gridMap.RadialSearch();

            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void PriorityQueueSearch_FindsNearestDrivers()
        {
            var gridMap = new GridMap(5, 5, 5);
            gridMap.GenerateMap();

            TestDelegate testDelegate = () => gridMap.PriorityQueueSearch();

            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void ShowMap_ExecutesWithoutErrors()
        {
            var gridMap = new GridMap(3, 3, 5);
            gridMap.GenerateMap();

            TestDelegate testDelegate = () => gridMap.ShowMap();

            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void CalcDistance_CalculatesManhattanDistanceCorrectly()
        {
            var gridMap = new GridMap(5, 5, 5);
            var cell1 = new MapCell(1, 1);
            var cell2 = new MapCell(4, 3);

            Assert.AreEqual(5, gridMap.CalcDistance(cell1, cell2)); // |4-1| + |3-1| = 3 + 2 = 5
        }

        [Test]
        public void CalcDistance_SameCell_ReturnsZero()
        {
            var gridMap = new GridMap(5, 5, 5);
            var cell1 = new MapCell(2, 2);
            var cell2 = new MapCell(2, 2);

            Assert.AreEqual(0, gridMap.CalcDistance(cell1,cell2));
        }

        [Test]
        public void FindNeighbors_ReturnsCorrectNumberOfDrivers()
        {
            var gridMap = new GridMap(5, 5, 5);
            var allDrivers = new List<MapCell>
            {
                new MapCell(0, 0, true) { indentificator = 1 },
                new MapCell(1, 1, true) { indentificator = 2 },
                new MapCell(2, 2, true) { indentificator = 3 },
                new MapCell(3, 3, true) { indentificator = 4 },
                new MapCell(4, 4, true) { indentificator = 5 }
            };
            var order = new MapCell(2, 2);

            Assert.AreEqual(5, gridMap.FindNeighbors(allDrivers, order).Count);
        }

        [Test]
        public void FindNeighbors_LessThan5Drivers_ReturnsAllAvailable()
        {
            var gridMap = new GridMap(5, 5, 5);
            var allDrivers = new List<MapCell>
            {
                new MapCell(0, 0, true) { indentificator = 1 },
                new MapCell(1, 1, true) { indentificator = 2 },
                new MapCell(2, 2, true) { indentificator = 3 }
            };
            var order = new MapCell(2, 2);

            Assert.AreEqual(3, gridMap.FindNeighbors(allDrivers, order).Count);
        }

        [Test]
        public void FindNeighbors_ReturnsSortedByDistance()
        {
            // Arrange
            var gridMap = new GridMap(5, 5, 5);
            var allDrivers = new List<MapCell>
            {
                new MapCell(4, 4, true) { indentificator = 1 }, // дальше всего
                new MapCell(1, 1, true) { indentificator = 2 }, // ближе
                new MapCell(0, 0, true) { indentificator = 3 }, // дальше
                new MapCell(2, 2, true) { indentificator = 4 }, // ближе всего
                new MapCell(3, 3, true) { indentificator = 5 }  // дальше
            };
            var order = new MapCell(2, 2);

            //проверяем что первый в списке ближайший
            Assert.AreEqual(4, gridMap.FindNeighbors(allDrivers, order)[0].indentificator);
        }

        [Test]
        public void GenerateMap_MinimumValidParameters_WorksCorrectly()
        {
            var gridMap = new GridMap(3, 3, 5);

            Assert.DoesNotThrow(() => gridMap.GenerateMap());
        }
    }

    [TestFixture]
    public class SearchAlgorithmsSimpleTests
    {
        [Test]
        public void AllSearchMethods_ReturnSameFiveDrivers()
        {
            var gridMap = new GridMap(10, 10, 15);
            gridMap.GenerateMap();

            var enumResult = gridMap.EnumerationSearch();
            var radialResult = gridMap.RadialSearch();
            var priorityResult = gridMap.PriorityQueueSearch();

            Assert.AreEqual(5, enumResult.Count);
            Assert.AreEqual(5, radialResult.Count);
            Assert.AreEqual(5, priorityResult.Count);
        }


        [Test]
        public void AllSearchMethods_ReturnSameDistances()
        {
            var gridMap = new GridMap(10, 10, 10);
            gridMap.GenerateMap();

            var enumResult = gridMap.EnumerationSearch();
            var radialResult = gridMap.RadialSearch();
            var priorityResult = gridMap.PriorityQueueSearch();

            //проверяем что расстояния до всех водителей одинаковые
            for (int i = 0; i < 5; i++)
            {
                var enumDistance = gridMap.CalcDistance(enumResult[i], gridMap.orderReciever);
                var radialDistance = gridMap.CalcDistance(radialResult[i], gridMap.orderReciever);
                var priorityDistance = gridMap.CalcDistance(priorityResult[i], gridMap.orderReciever);

                Assert.AreEqual(enumDistance, radialDistance);
                Assert.AreEqual(enumDistance, priorityDistance);
            }
        }

        [Test]
        public void AllSearchMethods_ReturnSameDistancesAnyCase()
        {
            Random rnd = new Random();

            for (int n = 5; n < 100; n++)
            {
                for (int m = 5; m < 100;m++)
                {
                    var gridMap = new GridMap(n, m, rnd.Next(5,(n*m)/2));
                    gridMap.GenerateMap();

                    var enumResult = gridMap.EnumerationSearch();
                    var radialResult = gridMap.RadialSearch();
                    var priorityResult = gridMap.PriorityQueueSearch();

                    for (int i = 0; i < 5; i++)
                    {
                        var enumDistance = gridMap.CalcDistance(enumResult[i], gridMap.orderReciever);
                        var radialDistance = gridMap.CalcDistance(radialResult[i], gridMap.orderReciever);
                        var priorityDistance = gridMap.CalcDistance(priorityResult[i], gridMap.orderReciever);

                        Assert.AreEqual(enumDistance, radialDistance);
                        Assert.AreEqual(enumDistance, priorityDistance);
                    }
                }
            }
        }

        [Test]
        public void SearchMethods_WorkWithMinimumRequiredDrivers()
        {
            var gridMap = new GridMap(6, 6, 5); 
            gridMap.GenerateMap();

            var enumResult = gridMap.EnumerationSearch();
            var radialResult = gridMap.RadialSearch();
            var priorityResult = gridMap.PriorityQueueSearch();

            Assert.AreEqual(5, enumResult.Count);
            Assert.AreEqual(5, radialResult.Count);
            Assert.AreEqual(5, priorityResult.Count);
        }

    }

    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        public void AllSearchMethods_ProduceConsistentResults()
        {
            var gridMap = new GridMap(8, 8, 10);
            gridMap.GenerateMap();

            //проверяем что все методы поиска работают без ошибок
            Assert.DoesNotThrow(() => gridMap.EnumerationSearch());
            Assert.DoesNotThrow(() => gridMap.RadialSearch());
            Assert.DoesNotThrow(() => gridMap.PriorityQueueSearch());
        }

        [Test]
        public void LargeMap_PerformanceTest()
        {
            var gridMap = new GridMap(50, 50, 20);
            gridMap.GenerateMap();

            Assert.DoesNotThrow(() => gridMap.EnumerationSearch());
            Assert.DoesNotThrow(() => gridMap.RadialSearch());
            Assert.DoesNotThrow(() => gridMap.PriorityQueueSearch());
        }
    }
}