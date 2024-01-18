using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;
using System.Diagnostics;

namespace flying_postman_pat
{
    class Program
    {

        static void Main(string[] args)
        {
            //General variables and initialisation
            string fileName = "";
            string filePlane = "";
            string startTime = "";
            string flag = "";
            string outputName = "";
            bool output = false;


            //Checking the amount of arguments that are provided to the program and to output it or not.
            if (args.Length < 3)
            {
                WriteLine("Error! Not enough arguments provided.");
            }
            else
            {
                fileName = args[0];
                filePlane = args[1];
                startTime = args[2];
            }

            if (args.Length > 3)
            {
                flag = args[3];
                outputName = args[4];
                output = true;
            }

            //Left to right (: is ignored)
            int[] startTimeFormat = new int[4];

            startTimeFormat[0] = (int)Char.GetNumericValue(startTime[0]);
            startTimeFormat[1] = (int)Char.GetNumericValue(startTime[1]);
            startTimeFormat[2] = (int)Char.GetNumericValue(startTime[3]);
            startTimeFormat[3] = (int)Char.GetNumericValue(startTime[4]);

            //Create objects and instantitate lists.
            Station station = new Station();
            Tour tour = new Tour();


            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();


            //Checking if file exists that was provided.
            if (File.Exists(fileName))
            {
                //Read the file contents of the station list and the plane specifications
                DoRead(fileName, station.name, station.xcoord, station.ycoord);
                ReadPlane(filePlane, Plane.plane.range, Plane.plane.speed, Plane.plane.takeOffTime, Plane.plane.landingTime, Plane.plane.refuelTime);

                //Begin time on how long algorithms take
                stopwatch.Start();
                Level1(station.name, tour.name, station.xcoord, tour.xcoord, station.ycoord, tour.ycoord);

                Level2(tour.name, tour.xcoord, tour.ycoord);
                stopwatch.Stop();
                //Output info to console
                DoConsole(outputName, tour.name, tour.xcoord, tour.ycoord, Plane.plane.range, Plane.plane.speed, Plane.plane.takeOffTime, Plane.plane.landingTime, Plane.plane.refuelTime, stopwatch.Elapsed.TotalSeconds,
                    startTimeFormat, output);
                //checking to see if the flag was provided and to output to a txt file.
                if (output)
                {
                    //Writes the the specified text file.
                    DoWrite(outputName, tour.name, tour.xcoord, tour.ycoord, Plane.plane.range, Plane.plane.speed, Plane.plane.takeOffTime,
                        Plane.plane.landingTime, Plane.plane.refuelTime, stopwatch.Elapsed.TotalSeconds, startTimeFormat);
                }
            }
            else
            {
                WriteLine("File does not exist");

            }
        }

        //Methods used to run the entire program.

        static void DoRead(string File, List<string> Namelist, List<int> xcoord, List<int> ycoord)
        {

            FileStream inFile = new FileStream(File, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);
            string recordIn;
            string[] fields;
            const char DELIM = ' ';
            recordIn = reader.ReadLine();
            while (recordIn != null)
            {
                fields = recordIn.Split(DELIM);

                Namelist.Add(fields[0]);
                xcoord.Add(Convert.ToInt32(fields[1]));
                ycoord.Add(Convert.ToInt32(fields[2]));

                recordIn = reader.ReadLine();
            }

            reader.Close();
            inFile.Close();


        }


        static void ReadPlane(string File, double Range, double Speed, int TakeOffTime, int LandingTime, int RefuelTime)
        {

            FileStream inFile = new FileStream(File, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);
            string recordIn;
            string[] fields;
            const char DELIM = ' ';
            recordIn = reader.ReadLine();
            while (recordIn != null)
            {
                fields = recordIn.Split(DELIM);

                Plane.plane.range = Convert.ToDouble(fields[0]);
                Plane.plane.speed = Convert.ToInt32(fields[1]);
                Plane.plane.takeOffTime = Convert.ToInt32(fields[2]);
                Plane.plane.landingTime = Convert.ToInt32(fields[3]);
                Plane.plane.refuelTime = Convert.ToInt32(fields[4]);

                recordIn = reader.ReadLine();

            }

            reader.Close();
            inFile.Close();

        }

        static double EuclidianDistance(int x1, int x2, int y1, int y2)
        {
            double Ed;
            Ed = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            return Ed;
        }

        //    Start with a list of stations e.g. [PO, TZ, XY, GP]
        //and an empty tour[]
        //for each station in stations:
        //    find spot to place station which results in least cost:
        //        if [special rules for first station [PO is always fixed] and second station[it only has one possible spot!]]
        //        else, iterate over each possible position: 
        //            find cost of adding at that position
        //    [end find]
        //    add at spot of least cost
        //[end for each]

        static void Level1(List<string> StationName, List<string> TourName, List<int> SxCoord, List<int> TxCoord, List<int> SyCoord, List<int> TyCoord)
        {
            int count = 0;
            double startDistance = 0;
            double thisDistance = 0;
            double tourDistance = 0;
            double bestDistance = 20000;
            int indexTrack = 0;
            foreach (string station in StationName)
            {
                if (station == StationName[0])
                {
                    TourName.Insert(0, station);
                    TxCoord.Insert(0, SxCoord[0]);
                    TyCoord.Insert(0, SyCoord[0]);

                    TourName.Insert(1, station);
                    TxCoord.Insert(1, SxCoord[0]);
                    TyCoord.Insert(1, SyCoord[0]);
                }
                else if (station == StationName[1])
                {
                    TourName.Insert(1, station);
                    TxCoord.Insert(1, SxCoord[1]);
                    TyCoord.Insert(1, SyCoord[1]);
                }

                else
                {
                    int size = TourName.Count;
                    startDistance = DoDistanceCheck(TourName, TxCoord, TyCoord);

                    for (int i = 1; i < size; i++)
                    {
                        tourDistance = startDistance;

                        //Minus the original two stations from total distance
                        tourDistance -= DoDistanceLeg(TxCoord[i - 1], TxCoord[i], TyCoord[i - 1], TyCoord[i]);

                        //TourName.Insert(i, station);
                        //TxCoord.Insert(i, SxCoord[count]);
                        //TyCoord.Insert(i, SyCoord[count]);
                        thisDistance = DoDistanceLeg(TxCoord[i - 1], SxCoord[count], TyCoord[i - 1], SyCoord[count]) + //calculating leg from s1 to insert.
                            DoDistanceLeg(SxCoord[count], TxCoord[i], SyCoord[count], TyCoord[i]); // calculating leg from insert to s2.

                        tourDistance += thisDistance;

                        //TourName.RemoveAt(i);
                        //TxCoord.RemoveAt(i);
                        //TyCoord.RemoveAt(i);

                        if (tourDistance < bestDistance)
                        {
                            bestDistance = tourDistance;
                            indexTrack = i;
                        }


                    }
                    TourName.Insert(indexTrack, station);
                    TxCoord.Insert(indexTrack, SxCoord[count]);
                    TyCoord.Insert(indexTrack, SyCoord[count]);
                    bestDistance = 20000;

                }
                count++;
            }
        }


        static void Level2(List<string> TourName, List<int> TxCoord, List<int> TyCoord)
        {

            int length = TourName.Count;
            double startDistance = 0;
            double thisDistance = 0;
            int indexTrack = 0;
            string nameBuffer = "";
            int xBuffer = 0;
            int yBuffer = 0;

            for (int i = 0; i < length; i++)
            {
                bool firstTime = true;
                bool newPos = false;

                int size = TourName.Count;
                startDistance = DoDistanceCheck(TourName, TxCoord, TyCoord);

                for (int j = 1; j < size - 1; j++)
                {
                    if (i > 0 && i < size - 1)
                    {

                        if (firstTime)
                        {
                            nameBuffer = TourName[i];
                            xBuffer = TxCoord[i];
                            yBuffer = TyCoord[i];

                            TourName.RemoveAt(i);
                            TxCoord.RemoveAt(i);
                            TyCoord.RemoveAt(i);
                            firstTime = false;
                        }

                        TourName.Insert(j, nameBuffer);
                        TxCoord.Insert(j, xBuffer);
                        TyCoord.Insert(j, yBuffer);

                        thisDistance = DoDistanceCheck(TourName, TxCoord, TyCoord);

                        if (thisDistance < startDistance)
                        {
                            startDistance = thisDistance;
                            indexTrack = j;
                            newPos = true;
                        }
                        TourName.RemoveAt(j);
                        TxCoord.RemoveAt(j);
                        TyCoord.RemoveAt(j);
                    }
                    else
                    {
                        break;
                    }
                    if (newPos)
                    {
                        TourName.Insert(indexTrack, nameBuffer);
                        TxCoord.Insert(indexTrack, xBuffer);
                        TyCoord.Insert(indexTrack, yBuffer);
                        i = 0;
                        break;
                    }
                    else if (j == size - 2)
                    {
                        TourName.Insert(i, nameBuffer);
                        TxCoord.Insert(i, xBuffer);
                        TyCoord.Insert(i, yBuffer);
                    }
                }
            }
        }


        static double DoDistanceLeg(int x1, int x2, int y1, int y2)
        {
            double leg;
            leg = EuclidianDistance(x1, x2, y1, y2);
            return leg;
        }

        static double DoDistanceCheck(List<string> names, List<int> xcoords, List<int> ycoords) // Will check the total distance of the current tour.
        {
            int size = names.Count;
            double legDistance;
            double totalDistance = 0;

            for (int i = 0; i < size - 1; i++)
            {
                legDistance = EuclidianDistance(xcoords[i], xcoords[i + 1], ycoords[i], ycoords[i + 1]);
                totalDistance += legDistance;
            }
            return totalDistance;
        }

        static void DoConsole(string FileName, List<string> names, List<int> xcoords, List<int> ycoords, double range, double speed, int takeOff, int land, int refuel, double stopwatch, int[] startTime, bool output)
        {

            int size = names.Count;
            double[] legDistance = new double[names.Count - 1];
            double[] legTime = new double[names.Count - 1];
            double totalDistance = 0;
            double totalTourTime = 0;
            double totalTimeMins = 0;
            double totalTimeHours = 0;
            double totalTimeDays = 0;

            speed = (double)speed / 60;
            stopwatch = Math.Round(stopwatch, 3);

            WriteLine("Optimising tour length: Level 2...");
            WriteLine("Elapsed time: {0} seconds", stopwatch);

            double legTimeTotal = 0;
            int count = 1;
            for (int i = 0; i < size - 1; i++)
            {
                legDistance[i] = EuclidianDistance(xcoords[i], xcoords[i + 1], ycoords[i], ycoords[i + 1]);
                totalDistance += legDistance[i];
                legTime[i] = Math.Round((legDistance[i] / speed) + (takeOff + land));
                totalTourTime += legTime[i];

                if (i > 0)
                {
                    if (legTime[i] + legTimeTotal > (range * 60.0))
                    {
                        count++;
                        legTimeTotal = 0;

                    }
                    else
                    {
                        legTimeTotal = legTime[i] + legTimeTotal;
                    }
                }
                else
                {
                    legTimeTotal += legTime[i];
                }


            }

            totalTourTime += count * refuel;

            totalDistance = Math.Round(totalDistance, 4);
            WriteLine("Tour length: {0}", totalDistance); //do station to station and calculate times.

            if (totalTourTime > 60)
            {
                totalTourTime = totalTourTime / 60;
                totalTimeHours = Math.Truncate(totalTourTime);
                totalTimeMins = Math.Round((totalTourTime - totalTimeHours) * 60);

                if (totalTimeHours > 23)
                {
                    totalTourTime = totalTimeHours / 24;
                    totalTimeDays = Math.Truncate(totalTourTime);
                    totalTimeHours = Math.Round((totalTourTime - totalTimeDays) * 24.0);
                    WriteLine("Tour time: {0} days {1} hours {2} minutes", totalTimeDays, totalTimeHours, totalTimeMins);
                }
                else
                {
                    WriteLine("Tour time: {0} hours {1} minutes", totalTimeHours, totalTimeMins);
                }

            }


            totalDistance = 0;
            double storedLegTime = 0;
            bool reset = false;

            for (int i = 0; i < size - 1; i++)
            {

                if (reset)
                {
                    storedLegTime = 0;
                }

                if (i > 0)
                {
                    if (legTime[i] + storedLegTime > (range * 60.0))
                    {
                        WriteLine("*** refuel {0} minutes ***", refuel);
                        reset = true;
                    }
                    else
                    {
                        storedLegTime = legTime[i] + storedLegTime;
                        reset = false;
                    }
                }

                else
                {
                    storedLegTime = legTime[i];

                    WriteLine("{0} 	 -> 	 {1} 	 {2}{3}:{4}{5} 	 {6}{7}:{8}{9}", names[i], names[i + 1],
                    startTime[0], startTime[1], startTime[2], startTime[3],
                    startTime[0], startTime[1], startTime[2], startTime[3]);
                }

                WriteLine("{0} 	 -> 	 {1} 	 {2}{3}:{4}{5} 	 {6}{7}:{8}{9}", names[i], names[i + 1],
                startTime[0], startTime[1], startTime[2], startTime[3],
                startTime[0], startTime[1], startTime[2], startTime[3]);
            }

            if (output)
            {
                WriteLine("Saving itinerary to {0}", FileName);
            }


        }




        static void DoWrite(string FileName, List<string> names, List<int> xcoords, List<int> ycoords, double range, double speed, int takeOff, int land, int refuel, double stopwatch, int[] startTime)
        {

            int size = names.Count;
            double[] legDistance = new double[names.Count - 1];
            double[] legTime = new double[names.Count - 1];
            double totalDistance = 0;
            double totalTourTime = 0;
            double totalTimeMins = 0;
            double totalTimeHours = 0;
            double totalTimeDays = 0;

            speed = (double)speed / 60;
            stopwatch = Math.Round(stopwatch, 3);


            FileStream outFile = new FileStream(FileName, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(outFile);
            writer.WriteLine("Optimising tour length: Level 2...");
            writer.WriteLine("Elapsed time: {0} seconds", stopwatch);

            double legTimeTotal = 0;
            int count = 1;
            for (int i = 0; i < size - 1; i++)
            {
                legDistance[i] = EuclidianDistance(xcoords[i], xcoords[i + 1], ycoords[i], ycoords[i + 1]);
                totalDistance += legDistance[i];
                legTime[i] = Math.Round((legDistance[i] / speed) + (takeOff + land));
                totalTourTime += legTime[i];

                if (i > 0)
                {
                    if (legTime[i] + legTimeTotal > (range * 60.0))
                    {
                        count++;
                        legTimeTotal = 0;

                    }
                    else
                    {
                        legTimeTotal = legTime[i] + legTimeTotal;
                    }
                }
                else
                {
                    legTimeTotal += legTime[i];
                }


            }

            totalTourTime += count * refuel;

            totalDistance = Math.Round(totalDistance, 4);
            writer.WriteLine("Tour length: {0}", totalDistance); //do station to station and calculate times.

            if (totalTourTime > 60)
            {
                totalTourTime = totalTourTime / 60;
                totalTimeHours = Math.Truncate(totalTourTime);
                totalTimeMins = Math.Round((totalTourTime - totalTimeHours) * 60);

                if (totalTimeHours > 23)
                {
                    totalTourTime = totalTimeHours / 24;
                    totalTimeDays = Math.Truncate(totalTourTime);
                    totalTimeHours = Math.Round((totalTourTime - totalTimeDays) * 24.0);
                    writer.WriteLine("Tour time: {0} days {1} hours {2} minutes", totalTimeDays, totalTimeHours, totalTimeMins);
                }
                else
                {
                    writer.WriteLine("Tour time: {0} hours {1} minutes", totalTimeHours, totalTimeMins);
                }

            }


            totalDistance = 0;
            double storedLegTime = 0;
            bool reset = false;
            double[] nextLegTime = new double[4];

            for (int i = 0; i < size - 1; i++)
            {


                if (reset)
                {
                    storedLegTime = 0;
                }

                if (i > 0)
                {
                    if (legTime[i] + storedLegTime > (range * 60.0))
                    {
                        writer.WriteLine("*** refuel {0} minutes ***", refuel);
                        reset = true;
                    }
                    else
                    {
                        storedLegTime = legTime[i] + storedLegTime;
                        reset = false;
                    }
                }

                else
                {
                    storedLegTime = legTime[i];



                    writer.WriteLine("{0} 	 -> 	 {1} 	 {2}{3}:{4}{5} 	 {6}{7}:{8}{9}", names[i], names[i + 1],
                        startTime[0], startTime[1], startTime[2], startTime[3],
                        startTime[0], startTime[1], startTime[2], startTime[3]);
                }

                writer.WriteLine("{0} 	 -> 	 {1} 	 {2}{3}:{4}{5} 	 {6}{7}:{8}{9}", names[i], names[i + 1],
                    startTime[0], startTime[1], startTime[2], startTime[3],
                    startTime[0], startTime[1], startTime[2], startTime[3]);
            }

            writer.Close();
            outFile.Close();
        }


    }

}
