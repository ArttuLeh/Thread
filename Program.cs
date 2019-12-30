/**********************************
 *                                *
 * Arttu Lehtovaara               *
 *                                *
 **********************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace thread_version
{
    class Program
    {
        static int products_lenght;
        static int thread_count;
        static int[] products;
        static Mutex mutex = new Mutex();

        static int findFreeIndex(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        static void producer(object data)
        {
            // thread id will be 1, 2... (delivered in Thread.Start(i + 1))
            
            int tid = (int)data;

            while (true)
            {
                // lock mutex
                mutex.WaitOne();
                
                int index = findFreeIndex(products);
                // if found --> write my tid to found index
                if (index >= 0)
                {
                    products.SetValue(tid, index);
                }
                mutex.ReleaseMutex();
                //if not found-- > break
                if (index == -1)
                    break;
                Thread.Sleep(100);
            }
        }

        static void Main(string[] args)
        {
            // check first the args count
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: <prog> <product_count> <thread_count>");
                Console.ReadLine();
                return;
            }
            // convert args to int.. use default values if illegal
            if (!int.TryParse(args[0], out products_lenght))
            {
                products_lenght = 100;
            }
            if (!int.TryParse(args[1], out thread_count))
            {
                thread_count = 10;
            }
            // now we have legal values, let's create the array
            products = new int[products_lenght];
            // let's create the threads..
            Thread[] threads = new Thread[thread_count];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(producer);
            }
            // and run the threads
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Start(i + 1);
            }
            // and wait for their finish
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }
            // and finally calculate statistics
            int[] products_count = new int[thread_count + 1];
            
            // for loop where all the products counted by producer id
            for (int i = 0; i < products.Length; i++)
            {
                products_count[products[i]]++;
            }
            foreach (var item in products)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
            for (int i = 0; i < products_count.Length; i++)
            {
                if (i == 0)
                {
                    Console.Write("- ");
                }
                else
                {
                    Console.Write(products_count[i] + " ");
                }
            }
            Console.WriteLine();
        }
    }
}
