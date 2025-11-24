using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;

namespace Practice_1_
{

    public class MapCell
    {
        public int x { get; set; }
        public int y { get; set; }
        public bool isDriver { get; set; }
        public bool isOrder { get; set; }
        public int indentificator { get; set; }


        public MapCell(int x, int y, bool isDriver)
        {
            if (x < 0) throw new ArgumentException("Значение x должно быть положительным", nameof(x));
            if (y < 0) throw new ArgumentException("Значение y должно быть положительным", nameof(x));
            this.x = x;
            this.y = y;
            this.isDriver = isDriver;
        }

        public MapCell(int x, int y)
        {
            if (x < 0) throw new ArgumentException("Значение x должно быть положительным", nameof(x));
            if (y < 0) throw new ArgumentException("Значение y должно быть положительным", nameof(x));
            this.x = x;
            this.y = y;
        }
    }

    public class GridMap
    {
        MapCell[,] mapCells;
        Random random;
        public MapCell orderReciever;
        private int n;
        private int m;
        private int drivers;

        public GridMap(int n, int m, int drivers)
        {
            if (n <= 0) throw new ArgumentException("Размер n должен быть положительным числом", nameof(n));
            if (m <= 0) throw new ArgumentException("Размер m должен быть положительным числом", nameof(m));
            if (drivers < 0) throw new ArgumentException("Количество водителей не может быть отрицательным", nameof(drivers));
            this.n = n;
            this.m = m;
            this.drivers = drivers;
            this.random = new Random();
        }

        public MapCell[,] GenerateMap()
        {
            if (n * m < drivers + 1) throw new Exception("Слишком маленькая карта, пожалуйста, укажите большие значения");
            if (drivers < 5) throw new Exception("Должно быть указано минимум 5 водителей");

            mapCells = new MapCell[n, m];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    mapCells[i, j] = new MapCell(i, j);
                }
            }
            DriversGenerator(mapCells, drivers);
            OrderGeneration(mapCells);
            return mapCells;
        }

        public void ShowMap()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    var cell = mapCells[i, j];

                    if (cell.isDriver)
                    {
                        Console.Write(String.Format("[ {0} ] ", cell.indentificator));
                    }
                    else if (cell.isOrder)
                    {
                        Console.Write("[***] ");
                    }
                    else
                    {
                        Console.Write(String.Format("[{0},{1}] ", cell.x, cell.y));
                    }
                }
                Console.WriteLine();
            }
        }

        //Поиск через простой перебор точек карты
        public List<MapCell> EnumerationSearch()
        {
            List<MapCell> allDrivers = new List<MapCell>();
            List<MapCell> total;
            foreach (var cell in mapCells)
            {
                if (cell.isDriver)
                {
                    allDrivers.Add(cell);
                }
            }
            total = FindNeighbors(allDrivers, orderReciever);
            Console.WriteLine();
            return total;
        }

        //Увеличиваем радиус-квадрат поиска от заказа, пока не насчитаем 5 водителей
        public List<MapCell> RadialSearch()
        {
            int curDrivers = 0;
            List<MapCell> allDrivers = new List<MapCell>();
            int maxRadius = Math.Max(n,m);
            for (int i = 1; i < maxRadius; i++)
            {
                //Регулировка по высоте
                for (int j = -i; j <= i; j++)
                {
                    //Проверка выхода за границы
                    if (mapCells[orderReciever.x, orderReciever.y].x + j >= n || 
                        mapCells[orderReciever.x, orderReciever.y].x + j < 0)
                    {
                        continue;
                    }
                    //Регулировка по ширине
                    for (int w = -i; w <= i; w++)
                    {
                        //Проверка выхода за границы
                        if (mapCells[orderReciever.x, orderReciever.y].y + w >= m || 
                            mapCells[orderReciever.x, orderReciever.y].y + w < 0)
                        {
                            continue;
                        }
                        //Учитываем только новые ячейки по периметру, проверенные не обходим
                        if(-i < j && i > j)
                        {
                            if (orderReciever.y + i < m && mapCells[orderReciever.x + j, orderReciever.y + i].isDriver)
                            {
                                curDrivers++;
                                allDrivers.Add(mapCells[orderReciever.x + j, orderReciever.y + i]);
                            }

                            if (orderReciever.y - i >= 0 && mapCells[orderReciever.x + j, orderReciever.y - i].isDriver)
                            {
                                curDrivers++;
                                allDrivers.Add(mapCells[orderReciever.x + j, orderReciever.y - i]);
                            }
                            break;
                        }
                        if (mapCells[orderReciever.x + j, orderReciever.y + w].isDriver)
                        {
                            curDrivers++;
                            allDrivers.Add(mapCells[orderReciever.x + j, orderReciever.y + w]);
                        }
                    }
                }
                //Если найдено 5 и более водителей
                if (curDrivers >= 5)
                {
                    List<MapCell> total = FindNeighbors(allDrivers, orderReciever);
                    //Если расстояние до заказа больше радиуса итерации, возможно наличие более близкого водителя на большем радиусе i
                    if (CalcDistance(total.Last(), orderReciever) > i && i != maxRadius-1)
                    {
                        continue;
                    }
                    Console.WriteLine();
                    return total;
                }
            }
            return null;
        }

        //Поиск через приоритетную очередь
        public List<MapCell> PriorityQueueSearch()
        {
            List<MapCell> allDrivers = new List<MapCell>();
            foreach (var cell in mapCells)
            {
                if (cell.isDriver)
                {
                    allDrivers.Add(cell);
                }
            }
            PriorityQueue<MapCell, int> pq = new PriorityQueue<MapCell, int>();

            foreach (var driver in allDrivers)
            {
                int distance = CalcDistance(driver, orderReciever);
                pq.Enqueue(driver, distance);
            }


            List<MapCell> nearestDrivers = new List<MapCell>();
            for (int i = 0; i < 5; i++)
            {
                nearestDrivers.Add(pq.Dequeue());
            }
            Console.WriteLine();

            return nearestDrivers;
        }
        public void DriversGenerator(MapCell[,] mapCells, int dCount)
        {
            int drivers = 0;
            int rn_x;
            int rn_y;
            while (drivers < dCount)
            {
                rn_x = random.Next(0, n);
                rn_y = random.Next(0, m);
                if (!mapCells[rn_x, rn_y].isDriver)
                {
                    mapCells[rn_x, rn_y].isDriver = true;
                    mapCells[rn_x, rn_y].indentificator = drivers;
                    drivers++;
                }

            }
        }
        public void OrderGeneration(MapCell[,] mapCells)
        {
            int order = 0;
            int rn_x;
            int rn_y;
            while (order != 1)
            {
                rn_x = random.Next(0, n);
                rn_y = random.Next(0, m);
                if (!mapCells[rn_x, rn_y].isDriver)
                {
                    mapCells[rn_x, rn_y].isOrder = true;
                    order++;
                    orderReciever = mapCells[rn_x, rn_y];
                }
            }
        }
        public int CalcDistance(MapCell cell1, MapCell cell2)
        {
            //Вычисляем Манхэттенское расстояние
            int distance = Math.Abs(cell2.x - cell1.x) + Math.Abs(cell2.y - cell1.y);
            return distance;
        }
        //Находит 5 ближайших водителей из всего списка
        public List<MapCell> FindNeighbors(List<MapCell> allDrivers, MapCell order)
        {
            List<MapCell> neighborsDrivers = new List<MapCell>();
            List<MapCell> driversCopy = new List<MapCell>(allDrivers); // Работаем с копией

            for (int i = 0; i < 5 && driversCopy.Count > 0; i++)
            {
                MapCell curMin = driversCopy[0];

                foreach (var cell in driversCopy)
                {
                    if (CalcDistance(cell, order) < CalcDistance(curMin, order))
                    {
                        curMin = cell;
                    }
                }

                neighborsDrivers.Add(curMin);
                driversCopy.Remove(curMin);
            }

            return neighborsDrivers;
        }

    }

    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class SearchAlgorithmsBenchmarks
    {
        private GridMap smallMap;
        private GridMap mediumMap;
        private GridMap largeMap;
        private GridMap veryLargeMap;

        [GlobalSetup]
        public void Setup()
        {
            //Маленькая карта (10x10)
            smallMap = new GridMap(10, 10, 8);
            smallMap.GenerateMap();

            //Средняя карта (50x50)
            mediumMap = new GridMap(50, 50, 25);
            mediumMap.GenerateMap();

            //Большая карта (100x100)
            largeMap = new GridMap(100, 100, 50);
            largeMap.GenerateMap();

            //Очень большая карта (200x200)
            veryLargeMap = new GridMap(200, 200, 100);
            veryLargeMap.GenerateMap();
        }

        //Маленькая карта
        [Benchmark]
        public void SmallMap_EnumerationSearch() => smallMap.EnumerationSearch();

        [Benchmark]
        public void SmallMap_RadialSearch() => smallMap.RadialSearch();

        [Benchmark]
        public void SmallMap_PriorityQueueSearch() => smallMap.PriorityQueueSearch();

        //Средняя карта
        [Benchmark]
        public void MediumMap_EnumerationSearch() => mediumMap.EnumerationSearch();

        [Benchmark]
        public void MediumMap_RadialSearch() => mediumMap.RadialSearch();

        [Benchmark]
        public void MediumMap_PriorityQueueSearch() => mediumMap.PriorityQueueSearch();

        //Большая карта
        [Benchmark]
        public void LargeMap_EnumerationSearch() => largeMap.EnumerationSearch();

        [Benchmark]
        public void LargeMap_RadialSearch() => largeMap.RadialSearch();

        [Benchmark]
        public void LargeMap_PriorityQueueSearch() => largeMap.PriorityQueueSearch();

        //Очень большая карта
        [Benchmark]
        public void VeryLargeMap_EnumerationSearch() => veryLargeMap.EnumerationSearch();

        [Benchmark]
        public void VeryLargeMap_RadialSearch() => veryLargeMap.RadialSearch();

        [Benchmark]
        public void VeryLargeMap_PriorityQueueSearch() => veryLargeMap.PriorityQueueSearch();
    }

    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class AlgorithmScenariosBenchmarks
    {
        private GridMap sparseDrivers;     //Мало водителей
        private GridMap denseDrivers;      //Много водителей
        private GridMap edgeCase;          //Пограничный случай

        [GlobalSetup]
        public void Setup()
        {
            //Мало водителей на большой карте
            sparseDrivers = new GridMap(100, 100, 10);
            sparseDrivers.GenerateMap();

            //Много водителей на маленькой карте
            denseDrivers = new GridMap(20, 20, 50);
            denseDrivers.GenerateMap();

            //Минимальные требования (5 водителей)
            edgeCase = new GridMap(10, 10, 5);
            edgeCase.GenerateMap();
        }

        //Мало водителей
        [Benchmark]
        public void SparseDrivers_EnumerationSearch() => sparseDrivers.EnumerationSearch();

        [Benchmark]
        public void SparseDrivers_RadialSearch() => sparseDrivers.RadialSearch();

        [Benchmark]
        public void SparseDrivers_PriorityQueueSearch() => sparseDrivers.PriorityQueueSearch();

        //Много водителей
        [Benchmark]
        public void DenseDrivers_EnumerationSearch() => denseDrivers.EnumerationSearch();

        [Benchmark]
        public void DenseDrivers_RadialSearch() => denseDrivers.RadialSearch();

        [Benchmark]
        public void DenseDrivers_PriorityQueueSearch() => denseDrivers.PriorityQueueSearch();

        //Пограничный случай
        [Benchmark]
        public void EdgeCase_EnumerationSearch() => edgeCase.EnumerationSearch();

        [Benchmark]
        public void EdgeCase_RadialSearch() => edgeCase.RadialSearch();

        [Benchmark]
        public void EdgeCase_PriorityQueueSearch() => edgeCase.PriorityQueueSearch();
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Выберите режим:");
            Console.WriteLine("1 - Бенчмарки (тестирование скорости)");
            Console.WriteLine("2 - Обычная работа программы");
            Console.Write("Введите 1 или 2: ");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.WriteLine("Запуск бенчмарков");
                var summary = BenchmarkRunner.Run<AlgorithmScenariosBenchmarks>();
            }
            else
            {
                // Обычная работа программы
                GridMap map = new GridMap(10, 8, 10);
                map.GenerateMap();
                map.ShowMap();
                var f1 = map.RadialSearch();
                var f2 = map.EnumerationSearch();
                var f3 = map.PriorityQueueSearch();
                foreach (var f in f1)
                {
                    Console.Write(String.Format("Ближайший водитель: id - {0}, расстояние до заказа - {1} км \n",
                    f.indentificator, map.CalcDistance(f, map.orderReciever)));
                }
                Console.WriteLine();
                foreach (var f in f2)
                {
                    Console.Write(String.Format("Ближайший водитель: id - {0}, расстояние до заказа - {1} км \n",
                    f.indentificator, map.CalcDistance(f, map.orderReciever)));
                }
                Console.WriteLine();
                foreach (var f in f3)
                {
                    Console.Write(String.Format("Ближайший водитель: id - {0}, расстояние до заказа - {1} км \n",
                    f.indentificator, map.CalcDistance(f, map.orderReciever)));
                }
            }
        }
    }
}