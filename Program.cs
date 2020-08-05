/*
 * Number Partition Problem
 * Created by: J78473
 * Last modified on: 01/23/2020
 * 
 * This application provides approximate solution on number partition problem 
 * Both uses Karmarkar-Karp Heuristic, for 2 partitions, 
 * and Greedy Algorithm, for more than 2
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Partition_Problem
{
    class Program
    {
        private static List<string> kkTreeList;
        public static void Main(string[] args)
        {
            bool isContinue = true;

            while (isContinue)
            {
                kkTreeList = new List<string>();
                isContinue = NumberPartition();
                Console.Clear();
            }
        }

        private static bool NumberPartition()
        {
            string universe = "";
            int partition;
            List<int> intUniverseList = new List<int>();

            try
            {
                Console.WriteLine("Number Partitioning Problem \n");
                Console.Write("Generate random universe? (Y/N): ");
                bool isRandom = Console.ReadLine().ToString().Trim().ToLower() == "y" ? true: false;

                if (!isRandom)
                {
                    Console.Write("Input universe (separated by space): ");
                    universe = Console.ReadLine();
                }
                else
                {
                    Console.Write("How many items?");
                    int countUniverse = Convert.ToInt16(Console.ReadLine());
                    universe = generateRandomUniverse(countUniverse);
                    Console.WriteLine("Generated universe: " + universe);
                }

                Console.Write("Input number of partition: ");
                partition = Convert.ToInt16(Console.ReadLine());

                intUniverseList = universe.Trim().Split(' ').Select(Int32.Parse).ToList();
            }
            catch (Exception ee)
            {
                Console.WriteLine("Invalid format on either the universe and partition. Kindly try again.");
                Console.ReadLine();
                return true;
            }

            //Initial checking (e.g. if sum is divisible to the number of partition)
            if (intUniverseList.Count < 1)
                Console.WriteLine("\nUniverse cannot be empty/null.");
            else if (intUniverseList.Count < partition)
                Console.WriteLine("\nPartition cannot be greater than the universe.");        
            else if(intUniverseList.Sum() % partition > 0)
                Console.WriteLine("\nNo perfect partition. But will proceed on giving approximate solution...");
            else
                Console.WriteLine("\nGiving approximate solution...");

            //approximate solution using Karmarkar-karp heuristic (for 2 partition)
            //and Greedy Algorithm (more than 2)
            if (partition == 2)
                KKHeuristic(intUniverseList);
            else
                GreedyAlgo(intUniverseList.ToArray(), partition);

            Console.WriteLine("\n\nWould you like to try again (Y/N)?");
            string isRetry = Console.ReadLine().ToString();
            if (isRetry.ToLower() == "y")
                return true;
            else if (isRetry.ToLower() == "n")
                return false;
            else
            {
                Console.WriteLine("Invalid input. Will terminate application.");
                Console.ReadLine();
                return false;
            }
        }

        /* Implements KARMARKAR-KARP HEURISTIC for 2 partitions
         * Initially, this will sort universe in descending order
         * then will get the difference of the first 2 large numbers before removing them on the list
         * the produced difference will then be included on the list (placing it in the correct order of the sorted list)
         * the same steps will happen until no or 1 item is left on the list. The item on the list, if any, will be the difference
         * of the two subsets
         * Also, during the process, a binary tree is generated for actual partitioning of subsets
         * 
         * Time complexity: O(nlogn) for worst case scenario
         * Space complexity: O(nlogn)
         */
        private static void KKHeuristic(List<int> intUniverseList)
        {
            intUniverseList.Sort((a, b) => b.CompareTo(a));            
            List<int> intUniverseListCopy = initializeList(intUniverseList.Count);

            while (intUniverseList.Count > 1)
            {
                updateTreeNodes(intUniverseList[0], intUniverseList[1], intUniverseListCopy[0], intUniverseListCopy[1]);

                int tempLargerNumber = intUniverseListCopy[0] == 0 ? intUniverseList[0] : intUniverseListCopy[0]; //this will be use for storing the copy
                int difference = intUniverseList[0] - intUniverseList[1];
                intUniverseList.RemoveAt(0);
                intUniverseList.RemoveAt(0);
                intUniverseListCopy.RemoveAt(0);
                intUniverseListCopy.RemoveAt(0);

                if (difference > 0 && intUniverseList.Count == 0)
                {
                    intUniverseList.Add(difference);
                    intUniverseListCopy.Insert(0, tempLargerNumber);
                }
                else if(difference > 0)
                {
                    //insert difference in the sorted list
                    for (int i = intUniverseList.Count - 1; 0 <= i; i--)
                    {
                        if (difference > intUniverseList[i])
                        {
                            if (i == 0)
                            {
                                intUniverseList.Insert(0, difference);
                                intUniverseListCopy.Insert(0, tempLargerNumber);
                            }
                            continue;
                        }
                        else
                        {
                            intUniverseList.Insert(i + 1, difference);
                            intUniverseListCopy.Insert(i + 1, tempLargerNumber);
                            break;
                        }
                    }
                }
            }

            displayKKSubsets(intUniverseList.Count > 0 ? intUniverseList[0] : 0);

            if (intUniverseList.Count == 0 || intUniverseList[0] == 0)
                Console.WriteLine("\n *****  Perfect partition exist. *****");
            else
            {
                Console.WriteLine("\n *****  No perfect partition exist. ***** \n");
                //Console.WriteLine("\nWill proceed on looking optimal solution....");
                //call optimal solution method
            }
        }

        /* Implements GREEDY ALGORITHM for more than 2 partitions
         * Initially, this will sort universe in descending order
         * then will place each number one by one to the subset which has a least value of sum
         * same process will happen until all numbers in the set are place
         * 
         * Time complexity: O(nlogn)
         * Space complexity: O(n)
         */
        private static void GreedyAlgo(int[] intUniverseArray, int partition)
        {
            Array.Sort(intUniverseArray);
            Array.Reverse(intUniverseArray);

            string[] arrSubset = new string[partition];
            formatSubset(arrSubset, true);

            int[] arrSum = new int[partition];

            foreach (var item in intUniverseArray)
            {
                int tmpLowestSum = arrSum[0];
                int tmpLowestIndex = 0;

                for (int i = 1; i < arrSum.Length; i++)
                {
                    if (arrSum[i] < tmpLowestSum)
                    {
                        tmpLowestSum = arrSum[i];
                        tmpLowestIndex = i;
                    }
                }

                arrSubset[tmpLowestIndex] += item + ", ";
                arrSum[tmpLowestIndex] += item;
            }

            formatSubset(arrSubset, false);
            Console.WriteLine("\n\nSubsets produced using Greedy Algorithm approximate solution: \n"
                + String.Join(" ", arrSubset));

            if (isEqual(arrSum))
                Console.WriteLine("\n ***** Perfect partition exist. *****\n");
            else
            {
                Console.WriteLine("\n***** No perfect partition exist. ***** \n");
                //Console.WriteLine("Will proceed on looking optimal solution....");
                //call optimal solution method
            }
        }


        /****** Methods for Karmarkar-karp Heuristic******/
        private static void displayKKSubsets(int difference)
        {
            /* Description:
             * this method will display the partition from the binary tree created during the KK process
             * e.g. from the 4,5,6,7,8 universe, final tree created will be 5-6-4-8-7
             *      this will then output subsets [5,4,7] [6,8] and difference 2
             */

            string subset1 = "{";
            string subset2 = "{";

            foreach(var item in kkTreeList)
            {
                int[] nums = Array.ConvertAll(item.Split('-'), int.Parse);

                for(int i = 0; i < nums.Length; i++)
                {
                    if ((i + 2) % 2 == 0)
                    {
                        subset1 += nums[i];
                        if (i < nums.Length)
                            subset1 += ", ";
                    }
                    else
                    {
                        subset2 += nums[i];
                        if (i < nums.Length)
                            subset2 += ", ";
                    }
                }
            }

            Console.WriteLine("\n\nSubsets produced using Karmarkar-Karp Heuristic approximate solution:" + 
                "\n" + subset1.Remove(subset1.Length - 2) + "} and " + subset2.Remove(subset2.Length - 2) + "}");

            if (difference > 0)
                Console.WriteLine("\nSubsets Difference: " + difference);
        }

        private static void hasOneExistingItem(int newVal, int oldVal)
        {
            /* Description:
             * this method will update the binary tree IF one item from the list is existing
             * e.g. binaryTreeList = {8-7} {6-5}
             * from the third iteration {4-8} will need to place in the treeList. but since 8 is existing,
             * just need to append 4 in {8-7} item, which will result into {4-8-7}
             */

            for (int i = 0; i < kkTreeList.Count; i++)
            {
                if (kkTreeList[i].Contains(oldVal.ToString() + "-"))
                {
                    kkTreeList[i] = newVal + "-" + kkTreeList[i];
                    break;
                }
                else if (kkTreeList[i].Contains(oldVal.ToString() + "-"))
                {
                    kkTreeList[i] = kkTreeList[i] + "-" + newVal;
                    break;
                }
            }
        }

        private static string hasBothExistingItem(string str1, string str2, bool bStr1, bool bStr2)
        {
            /* Description:
             * this method will update the binary tree IF both item from the list is existing
             * e.g. binaryTreeList = {4-8-7} {6-5}
             * from the fourth iteration {4-6} will need to place in the treeList. but since both items are existing,
             * just need to connect those two numbers which will result into {5-6-4-8-7}
             */

            //where number is located at the (1 - beginning; 0 - end)
            string newStr = "";

            if (bStr1 && bStr2)
            {
                str2 = reverseString(str2);
                newStr = concatString(str2, str1);
            }
            else if (bStr1 && !bStr2)
                newStr = concatString(str2, str1);
            else if (!bStr1 && bStr2)
                newStr = concatString(str1, str2);
            else if (!bStr1 && !bStr2)
            {
                str2 = reverseString(str2);
                newStr = concatString(str1, str2);
            }

            return newStr;
        }

        private static void updateTreeNodes(int iFirst, int iSecond, int iFirstCopy, int iSecondCopy)
        {
            /* Description:
             * this method will update the binary tree accordingly
             */

            //this will validate if number is existing in the tree by checking if the 'copy' variable is greater than 0
            bool bFirst = iFirstCopy > 0;
            bool bSecond = iSecondCopy > 0;

            //where number is (1 = existing; 0 = new) from the list
            if (!bFirst && !bSecond)
            {
                kkTreeList.Add(iFirst + "-" + iSecond);
            }
            else if (bFirst && !bSecond)
            {
                hasOneExistingItem(iSecond, iFirstCopy);
            }
            else if (!bFirst && bSecond)
            {
                hasOneExistingItem(iFirst, iSecondCopy);
            }
            else if (bFirst && bSecond)
            {
                bool bFirstLocation = true;
                bool bSecondLocation = true;
                string str1 = "";
                string str2 = "";

                for (int i = 0; i < kkTreeList.Count; i++)
                {
                    bool isFound = false;

                    if (kkTreeList[i].Contains(iFirstCopy + "-"))
                    {
                        bFirstLocation = true;
                        str1 = kkTreeList[i];
                        isFound = true;
                    }
                    else if (kkTreeList[i].Contains("-" + iFirstCopy))
                    {
                        bFirstLocation = false;
                        str1 = kkTreeList[i];
                        isFound = true;
                    }
                    else if (kkTreeList[i].Contains(iSecondCopy + "-"))
                    {
                        bSecondLocation = true;
                        str2 = kkTreeList[i];
                        isFound = true;
                    }
                    else if (kkTreeList[i].Contains("-" + iSecondCopy))
                    {
                        bSecondLocation = false;
                        str2 = kkTreeList[i];
                        isFound = true;
                    }

                    if(isFound)
                    {
                        kkTreeList.RemoveAt(i);
                        i--;
                    }
                }

                if (str1 != "" || str2 != "")
                    kkTreeList.Add(hasBothExistingItem(str1, str2, bFirstLocation, bSecondLocation));
                else
                    Console.WriteLine("unable to find number on tree: updateTreeNodes");
            }
        }       

        private static string reverseString(string str)
        {
            /* Description:
             * reverse the string provided having "-" as the separator
             */

            int[] nums = Array.ConvertAll(str.Split('-'), int.Parse);
            Array.Reverse(nums);
            return (string.Join("-", nums));
        }

        private static string concatString(string str1, string str2)
        {
            return (str1 + "-" + str2);
        }
        
        private static List<int> initializeList(int index)
        {
            /* Description:
             * this method will initialize 'copy' variable and will assign 0 by default
             * 'copy' variable will be a temporary holder of the original number of the produced difference, if applicable
             * 0 means it has the original and need to check the universe list
             * > 0 means the original number
             * 
             * e.g. by default
             *      universe = {8,7,6,5,4}
             *      copy = {0,0,0,0,0}
             * 
             *      complete universe with the difference on it using KK
             *      universe = {8,7,6,5,4,1,1}
             *      copy = {0,0,0,0,0,8,6}
             *      wherein both 1 on the universe is from the remaining numbers of 8 and 6
             */

            List<int> list = new List<int>();

            for(int i = 0; i < index; i++)
            {
                list.Add(0);
            }

            return list;
        }
        /****** End: Methods for Karmarkar-karp Heuristic******/

        /****** Methods for Greedy Algorithm ******/
        private static string[] formatSubset(string[] arr, bool isBeginning)
        {
            /* Description:
             * this method will add "{" if start and "}" if end on each subsets
             */

            if (isBeginning)
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = "{";
                }
            else
                for (int i = 0; i < arr.Length; i++)
                {                    
                    arr[i] = arr[i].Remove(arr[i].Length - 2) + "}";
                }

            return (arr);
        }

        private static bool isEqual(int[] arr)
        {
            /* Description:
             * this method will return true if all the sum of subsets are equal
             */

            bool isEqual = true;
            int baseNumber = arr[0];

            foreach (var item in arr)
            {
                if (item != baseNumber)
                {
                    isEqual = false;
                    break;
                }
            }

            return isEqual;
        }
        /****** End: Methods for Greedy Algorithm ******/

        private static string generateRandomUniverse(int count)
        {
            string universe = "";
            Random rnd = new Random();

            for(int i=0; i < count; i++)
            {
                universe += rnd.Next(1, 200) + " ";
            }

            return universe;
        }
    }
}