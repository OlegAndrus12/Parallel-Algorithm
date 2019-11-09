using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Matrix_Addition
{
    class Program
    {
        public static int M = 10000;
        public static int N = 10000;
        public static int k;
        public static int[,] A;
        public static int[,] B;
        public static int[,] AB;

        static void Main(string[] args)
        {

            A = new int[M, N];
            B = new int[N, N];
            AB = new int[N, N];

            A = read_from_file("C:\\Users\\User\\source\\repos\\Threading\\Matrix_Addition\\bin\\Debug\\MatrixA" + Convert.ToString(M) + ".txt", M, N);
            B = read_from_file("C:\\Users\\User\\source\\repos\\Threading\\Matrix_Addition\\bin\\Debug\\MatrixB" + Convert.ToString(M) + ".txt", M, N);

            bool condition = true;
            while (condition)
            {
                Stopwatch stopwatch_parallel = new Stopwatch();
                Stopwatch stopwatch_simple = new Stopwatch();

                Console.WriteLine("How much threads do you want?");
                k = Convert.ToInt32(Console.ReadLine());
                
                //A = initialize_matrix(A, M, N); 
                //B = initialize_matrix(B, M, N);

                //write_to_file(A, M, N, "MatrixA500.txt");
                //write_to_file(B, M, N, "MatrixB500.txt");

                Thread[] threads = new Thread[k];

                int step = 0;
                stopwatch_parallel.Start();
                for (int i = 0; i < k; ++i)
                {
                    threads[i] = new Thread(new ParameterizedThreadStart(parallel_addition));
                    threads[i].Start(step);
                    step++;
                }

                for (int i = 0; i < k; ++i)
                {
                    threads[i].Join();
                }
                stopwatch_parallel.Stop();
                Console.WriteLine($"Execution time for parallel addition : {stopwatch_parallel.Elapsed}");

                //write_to_file(AB, M, N, "MatrixAB.txt");

                stopwatch_simple.Start();
                simple_addition(A, B, M, N);
                stopwatch_simple.Stop();
                Console.WriteLine($"Execution time for simple addition: {stopwatch_simple.Elapsed}");
                Console.WriteLine();
                Console.WriteLine("1 : # Again or  # 2 : enough?");
                string choice = Console.ReadLine();
                if (choice == "2")
                {
                    condition = false;
                }
            }
        }

        static int[,] initialize_matrix(int[,] A, int M, int N)
        {
            Random rand = new Random();
            for (int i = 0; i < M; ++i)
            {
                for (int j = 0; j < N; ++j)
                {
                    A[i, j] = rand.Next(0, 10);
                }
            }
            return A;
        }

        static int[,] simple_addition(int[,] A, int[,] B, int M, int N)
        {
            int[,] res = new int[M, N];
            for (int i = 0; i < M; ++i)
            {
                for (int j = 0; j < N; ++j)
                {
                    res[i, j] = A[i, j] + B[i, j];
                }
            }
            return res;
        }

        static void parallel_addition(object step)
        {
            int core = (int)step;
            for (int i = core * M / k; i < (core + 1) * M / k; i++)
            {
                for (int j = 0; j < M; ++j)
                {
                    AB[i, j] = A[i, j] + B[i, j];
                }
            }
        }

        static void display(int[,] Matrix, int M, int N)
        {
            for (int i = 0; i < M; ++i)
            {
                for (int j = 0; j < N; ++j)
                {
                    Console.Write(Matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        static void write_to_file(int[,] A, int M, int N, string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int i = 0; i < M; ++i)
                {
                    for (int j = 0; j < N; ++j)
                    {
                        sw.Write(A[i, j] + " ");
                    }
                    sw.WriteLine();
                }
            }
        }

        static int[,] read_from_file(string path, int M, int N)
        {
            string[] input1 = System.IO.File.ReadAllLines(path);
            int[,] res = new int[input1.Length, input1[0].Split(' ').Length - 1];
            for (int i = 0; i < input1.Length; ++i)
            {
                string[] to_add = input1[i].Split(' ');
                for (int j = 0; j < to_add.Length - 1; ++j)
                {
                    res[i, j] = Convert.ToInt32(to_add[j]);
                }
            }
            return res;
        }
    }
}
