﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
namespace AlgorytmyZPowracaniem
{
    class Program
    {
        static void GenerateAdjecencyMatrix(ref int[,] adjacencyMatrix, int saturate, int n) // generuje macierz sąsiedztwa
        {
            int x = 0, y = 0;
            int[] Tab = new int[n];
            Tab[0] = 1; // od niego zaczynamy
            for (int i = 1; i < n; i++) Tab[i] = 0;
            Random rand = new Random();

            for (int i = 0; i < n - 1; i++) // pętla generująca łańczusze początkowy -> Cykl Hamiltona
            {
                x = rand.Next(0, n); // losowanie kolumny
                if (Tab[x] == 0)
                {
                    adjacencyMatrix[y, x] = 1;  // Moment połączenia
                    adjacencyMatrix[x, y] = 1;  // Połączenie lustrzane

                    Tab[x] = 1; // Zaznaczenie w tablicy, że już połączone
                    y = x;
                }
                else
                {
                    i--;
                }
            }

            adjacencyMatrix[y, 0] = 1; // połącznie pierwszego i ostatniego węzła
            adjacencyMatrix[0, y] = 1; // połącznie pierwszego i ostatniego węzła


            // -- Generowanie połączonych trójek --
            int saturateUse = n; // licznik Saturate
            int a, b, c = 0; // punkty, które połączymy
            while (saturateUse < saturate)
            {
                a = rand.Next(0, n);
                do
                {
                    b = rand.Next(0, n);
                } while (b == a || adjacencyMatrix[a, b] == 1);

                do
                {
                    c = rand.Next(0, n);
                } while ((c == a || adjacencyMatrix[c, a] == 1) || (c == b || adjacencyMatrix[c, b] == 1));

                // --- Tworzenie połączeń ---
                adjacencyMatrix[a, c] = 1;
                adjacencyMatrix[c, a] = 1;

                adjacencyMatrix[a, b] = 1;
                adjacencyMatrix[b, a] = 1;

                adjacencyMatrix[c, b] = 1;
                adjacencyMatrix[b, c] = 1;

                saturateUse += 3; // zwiększenie saturate o 3 krawędzie
            }

            Console.Write("Udało się wylosować, jedziemy dalej \n");

            //// -- wyswietlanie --
            //Console.Write("    ");
            //for (int i = 0; i < n; i++) Console.Write(i + " ");
            //Console.Write("\n");
            //for (int i = 0; i < n; i++)
            //{
            //    Console.Write(i + " - ");
            //    for (int j = 0; j < n; j++)
            //    {
            //        Console.Write(adjacencyMatrix[i, j] + " ");
            //    }
            //    Console.Write("\n");
            //}
            //Console.Write("\n");

        }

        static void TransformToCommingList(ref int[,] adjacencyMatrix, ref List<int>[] commingList, int n) // zamiana na listę następników
        {
            for (int i = 0; i < n; i++) // iteracja po wierszach
            {
                //Console.Write(i + " => ");
                for (int j = 0; j < n; j++) // iteracja po kolumnach
                {
                    if (adjacencyMatrix[i, j] == 1) // szukanie zależności
                    {
                        commingList[i].Add(j);
                        //Console.Write(j + " -> ");
                    }
                }
                commingList[i].Add(-1); // znak końca
                //Console.Write("-1 \n");
            }
        }

        static void FindEuler(int node, ref List<int>[] commingList, ref List<int> solution)
        {
            int item = commingList[node].First();
            while (item != -1)
            {
                commingList[node].Remove(item);
                commingList[item].Remove(node); // usuwanie krawędzi

                FindEuler(item, ref commingList, ref solution); // wywołanie Eulera dla kolejnego elementu

                item = commingList[node].First();
            }
            solution.Add(node);
        }
        static void FindHamilton(int i, ref List<int>[] cominglist, ref List<int> solution, ref int[] visited, ref bool ham)
        {
            if (ham) return;
            visited[i] = 1;
            if ((solution.Count == visited.Length - 1) && (cominglist[i].Contains(solution[0]))) //pierwszy warunek na pewno daje sciezke, drugi dodatkowo daje cykl
            {
                ham = true;
              
                return; //jeśli istnieje krawędź miedzy i a solution[0] to przerywamy- mamy cykl

            }

            solution.Add(i); //zamiast tablicy stosujemy liste- dodajemy do niej wierzcholek
            foreach (int j in cominglist[i]) //dla kazdego nastepnika wierzcholka
            {
                if (j >= 0 && visited[j] != 1)
                {

                    FindHamilton(j, ref cominglist, ref solution, ref visited, ref ham); //jeśli następnika nie było w visited, wywołujemy rekurencyjnie
                }
            }

            solution.Remove(i);
            visited[i] = 0;
            return;
        }
   
        static void DeleteHamiltonFromMatrix(ref int[,] adjacencyMatrix, int n)
        {
            //wybieramy losowa wierzcholek i sprawiamy, że ma polaczenie z tylko jednym wierzcholkiem
            Random rnd = new Random();
            int f = rnd.Next(0, n-1); //wierzcholek odlaczany
            int x, y;
            Console.WriteLine(f);
            for(int i=0; i<n;i++)

            {
                if(adjacencyMatrix[i, f]== 1)
                {
                    do
                    {
                         x = rnd.Next(0, n - 1);
                         y = rnd.Next(0, n - 1);
                    } while (x == i || y == f||x==y);
                }
                if (adjacencyMatrix[f, i] == 1)
                {
                    do
                    {
                        x = rnd.Next(0, n - 1);
                        y = rnd.Next(0, n - 1);
                    } while (x == i || y == f || x == y);
                }

                adjacencyMatrix[i, f] = 0;
                adjacencyMatrix[f, i] = 0;
            }
   
        }

        static void Hamilton()
        {
            Stopwatch sw = new Stopwatch();
            //Hamilton
            bool ham = false;

           int n = 0;
            n = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("n=" + n + "\n");
            int m30 = (n * (n - 1) / 2) * 30 / 100; // ilość krawędzi dla nasycenia 30%
            int[,] adjacencyMatrix30 = new int[n, n]; // macierz sąsiedztwa 30%
            GenerateAdjecencyMatrix(ref adjacencyMatrix30, m30, n); // 30%

            int m70 = (n * (n - 1) / 2) * 70 / 100; // ilość krawędzi dla nasycenia 70%
            int[,] adjacencyMatrix70 = new int[n, n]; // macierz sąsiedztwa 70%
            GenerateAdjecencyMatrix(ref adjacencyMatrix70, m70, n); // 70%


            List<int>[] commingList30_Hamilton = new List<int>[n]; // Lista następników <- dla Kamila
            for (int i = 0; i < n; i++) commingList30_Hamilton[i] = new List<int>(); // inicjowanie obiektu
            TransformToCommingList(ref adjacencyMatrix30, ref commingList30_Hamilton, n); // 30%


            List<int>[] commingList70_Hamilton = new List<int>[n]; // Lista następników <- dla Kamila
            for (int i = 0; i < n; i++) commingList70_Hamilton[i] = new List<int>(); // inicjowanie obiektu
            TransformToCommingList(ref adjacencyMatrix70, ref commingList70_Hamilton, n); // 30%
                                                                                          //  tworzenie tablicy visited
            int[] visited = new int[n];
            List<int> solution = new List<int>(); // Lista wynikowa
            sw.Reset();
            sw.Restart();
            FindHamilton(0, ref commingList30_Hamilton, ref solution, ref visited, ref ham); // funkcja szukająca ciągu Hamiltona
            sw.Stop();
            solution.Clear();
            Console.WriteLine("Hamilton\t30%\t" + sw.ElapsedMilliseconds + "\n");
            visited = new int[n];
            sw.Reset();
            ham = false;
            sw.Start();
            FindHamilton(0, ref commingList70_Hamilton, ref solution, ref visited, ref ham);
            sw.Stop();
            Console.WriteLine("Hamilton\t70%\t" + sw.ElapsedMilliseconds + "\n");
            solution.Clear();

        }
        static void Main(string[] args)
        {
            
            //kod potrzebny do zapisu outputu konsoli do pliku
            //FileStream ostrm;
            //StreamWriter writer;
            //TextWriter oldOut = Console.Out;
            //ostrm = new FileStream("./Wyniki.txt", FileMode.OpenOrCreate, FileAccess.Write);
            //writer = new StreamWriter(ostrm);
            //Console.SetOut(writer);
            //CZESC A
            Stopwatch sw = new Stopwatch(); //inicjowanie pomiaru czasu
            int n;
            Console.WriteLine("Czesc A\n");

            ////Euler
            for (int x = 100; x <= 160; x += 6)
            {
                n = x; // liczba wierzchołków
                Console.WriteLine("n=" + n + "\n");
                int m30 = (n * (n - 1) / 2) * 30 / 100; // ilość krawędzi dla nasycenia 30%
                int[,] adjacencyMatrix30 = new int[n, n]; // macierz sąsiedztwa 30%
                GenerateAdjecencyMatrix(ref adjacencyMatrix30, m30, n); // 30%

                int m70 = (n * (n - 1) / 2) * 70 / 100; // ilość krawędzi dla nasycenia 70%
                int[,] adjacencyMatrix70 = new int[n, n]; // macierz sąsiedztwa 70%
                GenerateAdjecencyMatrix(ref adjacencyMatrix70, m70, n); // 70%


                // -- Euler --
                List<int>[] commingList30 = new List<int>[n]; // Lista następników
                for (int i = 0; i < n; i++) commingList30[i] = new List<int>(); // inicjowanie obiektu
                TransformToCommingList(ref adjacencyMatrix30, ref commingList30, n); // 30%

                Console.Write('\n');

                List<int>[] commingList70 = new List<int>[n]; // Lista następników
                for (int i = 0; i < n; i++) commingList70[i] = new List<int>(); // inicjowanie obiektu
                TransformToCommingList(ref adjacencyMatrix70, ref commingList70, n); // 70%

                // --- Algorytm szukania ---
                List<int> solution = new List<int>(); // Lista wynikowa
                sw.Reset();
                sw.Start();

                FindEuler(0, ref commingList30, ref solution); // funkcja szukająca ciągu Eulera
                sw.Stop();
                Console.WriteLine("Euler\t30%\t" + sw.ElapsedMilliseconds + "\n");
                solution.Clear();

                sw.Reset();
                sw.Start();
                FindEuler(0, ref commingList70, ref solution);
                sw.Stop();
                Console.WriteLine("Euler\t70%\t" + sw.ElapsedMilliseconds + "\n");
                solution.Clear();
                //   Console.Read();
            }
            Hamilton();

            // Sprawdzenie Hamiltona w grafie, w którym nie istnieje cykl hamiltona
            Console.WriteLine("Czesc B\n");
            bool ham = false;
            for (int i = -2; i < 1; i++)
            {
                n = 10 + i;
                int m50 = (n * (n - 1) / 2) * 50 / 100; // ilość krawędzi dla nasycenia 50%
                int[,] adjacencyMatrix50 = new int[n, n]; // macierz sąsiedztwa 50%
                GenerateAdjecencyMatrix(ref adjacencyMatrix50, m50, n); // 50%
                DeleteHamiltonFromMatrix(ref adjacencyMatrix50, n); //usuwamy cykl hamiltona z grafu
                List<int>[] commingList50_Hamilton = new List<int>[n]; // Lista następników <- dla Kamila
                for (int j = 0; j < n; j++) commingList50_Hamilton[j] = new List<int>(); // inicjowanie obiektu
                TransformToCommingList(ref adjacencyMatrix50, ref commingList50_Hamilton, n);
                int[] Visited = new int[n];
                List<int> Solution = new List<int>(); // Lista wynikowa


                sw.Reset();
                sw.Start();
                FindHamilton(0, ref commingList50_Hamilton, ref Solution, ref Visited, ref ham);
                sw.Stop();
                Console.WriteLine("n=" + n + ";Hamilton bez cyklu Hamiltona:" + sw.ElapsedMilliseconds + "\n");
            }
            //Console.SetOut(oldOut);
            //writer.Close();
            //ostrm.Close();
            Console.Read();
        }
    }
}
