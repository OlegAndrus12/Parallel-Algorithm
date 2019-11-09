using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Matrix_Multiplication
{
    public class Pair
    {
        public long start;
        public long end;
    }

    class Program
    {
        public static int M = 500;
        public static int k;
        public static int[,] A;
        public static int[,] B;
        public static int[,] AB;

        static void Main(string[] args)
        {
            A = new int[M, M];
            B = new int[M, M];
            AB = new int[M, M];

            A = read_from_file("C:\\Users\\User\\source\\repos\\Threading\\Matrix_Addition\\bin\\Debug\\MatrixA" + Convert.ToString(M) + ".txt");
            B = read_from_file("C:\\Users\\User\\source\\repos\\Threading\\Matrix_Addition\\bin\\Debug\\MatrixB" + Convert.ToString(M) + ".txt");

            //A = initialize_matrix(A, M);
            //B = initialize_matrix(B, M);

            bool condition = true;
            while (condition)
            {
                Stopwatch stopwatch1 = new Stopwatch();
                Stopwatch stopwatch2 = new Stopwatch();
                Stopwatch stopwatch3 = new Stopwatch();

                Console.WriteLine("How much threads do you want?");
                k = Convert.ToInt32(Console.ReadLine());
                long parts = M / k;
                Thread[] threads = new Thread[k];
                Pair pair;

                stopwatch1.Start();
                for (var i = 0; i < k; ++i)
                {

                    threads[i] = new Thread(new ParameterizedThreadStart(ComputeElements));
                    pair = new Pair
                    {
                        start = i * parts,
                        end = (i + 1) * parts
                    };
                    threads[i].Start(pair);
                }

                for (var i = 0; i < k; ++i)
                {
                    threads[i].Join();
                }
                stopwatch1.Stop();
                Console.WriteLine("Parallel multiplication (Thread) : " + stopwatch1.Elapsed);

                stopwatch3.Start();
                parallel_library();
                stopwatch3.Stop();
                Console.WriteLine("Parallel multiplication (Parallel) : " + stopwatch3.Elapsed);

                stopwatch2.Start();
                simple_multiplication(A, B, M);
                stopwatch2.Stop();
                Console.WriteLine("Simple multiplication : " + stopwatch2.Elapsed);
            }

        }

        static int[,] read_from_file(string path)
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

        static void display(int[,] Matrix, int M)
        {
            for (int i = 0; i < M; ++i)
            {
                for (int j = 0; j < M; ++j)
                {
                    Console.Write(Matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        static int[,] simple_multiplication(int[,] A, int[,] B, int M)
        {
            int[,] res = new int[M, M];
            for (int i = 0; i < M; ++i)
            {
                for (int j = 0; j < M; ++j)
                {
                    for (int k = 0; k < M; ++k)
                    {
                        AB[i, j] += A[i, k] * B[k, j];
                    }
                }
            }
            return res;
        }

        static int[,] initialize_matrix(int[,] A, int M)
        {
            Random rand = new Random();
            for (int i = 0; i < M; ++i)
            {
                for (int j = 0; j < M; ++j)
                {
                    A[i, j] = rand.Next(0, 10);
                }
            }
            return A;
        }

        static void parallel_multiplication(object obj)
        {
            Pair pair = (Pair)obj;
            for (long i = pair.start; i < pair.end; ++i)
            {
                for (long j = pair.start; j < pair.end; ++j)
                {
                    Pair pair1 = new Pair();
                    pair1.start = i;
                    pair1.end = j;
                    for (int z = 0; z < M; ++z)
                    {
                        AB[pair1.start, pair.end] = 0;
                        for (int q = 0; q < M; ++q)
                        {
                            AB[pair1.start, pair1.end] += A[pair1.start, q] * B[q, pair1.end];
                        }
                    }
                }
            }
        }

        static void parallel_library()
        {
            Parallel.For(0, M, i =>
            {
                for (int j = 0; j < M; ++j)
                {
                    for (int k = 0; k < M; ++k)
                    {
                        AB[i, j] += A[i, k] * B[k, j];
                    }
                }
            });
        }

        static void ComputeElement(Object _pair)
        {
            Pair pair = (Pair)_pair;
            AB[pair.start, pair.end] = 0;
            for (int i = 0; i < M; ++i)
            {
                AB[pair.start, pair.end] += A[pair.start, i] * B[i, pair.end];
            }
        }
        static void ComputeElements(Object _range)
        {
            Pair range = (Pair)_range;
            for (long i = range.start; i < range.end; ++i)
            {
                for (long j = range.start; j < range.end; ++j)
                {
                    Pair para = new Pair();
                    para.start = i;
                    para.end = j;
                    for (int k = 0; k < M; ++k)
                    {
                        // ComputeElement(para);
                        AB[i, j] += A[i, k] * B[k, j];
                    }
                }
            }
        }
    }
}
