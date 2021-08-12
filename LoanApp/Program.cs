using System;
using System.IO;
using System.Collections.Generic;

namespace LoanApp
{
    class Program
    {
        // Main function to call all other functions
        static void Main(string[] args)
        {
            // setting the file_path (change it here when you download the app
            string file_path = @"C:\Users\jenni\source\repos\LoanApp\LoanApp\loan-info.txt";
            // getting the user input
            Console.WriteLine("Enter the amount you need to loan");
            string temp1 = Console.ReadLine();
            Console.WriteLine("Enter the value of your collateral");
            string temp2 = Console.ReadLine();
            Console.WriteLine("Enter your credit score");
            string temp3 = Console.ReadLine();
            // converting the string input to ints so we can use it in calculations
            int loan_val = int.Parse(temp1); int collateral_val = int.Parse(temp2); int credit_score = int.Parse(temp3);
            // Running the calculations to determine if the loan will be approved
            int ltv = (int)Math.Round((double)(100 * loan_val) / collateral_val); // working out the loan to value %
            // approve or deny the loan, get and update the stats in the file and output to console
            bool loan_status = CalculateLoan(loan_val, collateral_val, credit_score, ltv);
            List<int> stats = GetCurrentStats(file_path);
            List<int> new_stats = UpdateStats(stats, file_path, loan_status, ltv, loan_val);
            OutputData(new_stats, loan_status);
        }

        // Function to work out whether a loan should be approved or denied
        static bool CalculateLoan(int loan_val, int collateral_val, int credit_score, int ltv)
        {
            bool loan_allowed = true; // bool to indicate loan status upon completion
            bool break_condition = false; // condition to break the while loop
            
            // iterate through the conditions, break if the loan is denied
            while (break_condition == false) {
                // condition 1: 1.5m > loan > 100k
                if (loan_val > 1500000 || loan_val < 100000)
                {
                    break_condition = true; loan_allowed = false;
                }
                // condition 2: is loan over 1m? If so is credit_score > 950 and ltv < 60
                if (loan_val > 1000000)
                {
                    if (credit_score < 950)
                    {
                        break_condition = true; loan_allowed = false;
                    }
                    else
                    {
                        if (ltv > 60)
                        {
                            break_condition = true; loan_allowed = false;
                        }
                    }
                }
                // condition 3: is loan under 1m apply the range rules 
                else
                {
                    //if ltv is 90% or more the application must be denied
                    if (ltv >= 90)
                    {
                        break_condition = true; loan_allowed = false;
                    }
                    // if the ltv is less than 60% 
                    else if (ltv < 60)
                    {
                        // the credit score must be 750 or more
                        if (credit_score < 750)
                        {
                            break_condition = true; loan_allowed = false;
                        }
                    }
                    // if the ltv is less than 80%
                    else if (ltv < 80 && ltv >= 60)
                    {
                        // the credit score must be 800 or more
                        if (credit_score < 800)
                        {
                            break_condition = true; loan_allowed = false;
                        }
                    }
                    // if the ltv is less than 90%
                    else if (ltv < 90 && ltv >= 80)
                    {
                        // the credit score must be 900 or more
                        if (credit_score < 900) {
                            break_condition = true; loan_allowed = false;
                        }
                    }
                }
                break_condition = true;
            }
            return loan_allowed; // return whether the loan was approved or not
        }
        // Function to read the file and return the data as vars
        static List<int> GetCurrentStats(string file_path)
        {
            // Initialising a streamreader
            StreamReader sr = new StreamReader(file_path);
            // reading through the lines in order
            // Line 1: total applicants
            string line = sr.ReadLine(); int total_apps = int.Parse(line);
            // Line 2: successful applicants
            string line2 = sr.ReadLine(); int successful_apps = int.Parse(line2);
            // Line 3: Total amount from all successful loans
            string line3 = sr.ReadLine(); int total_loan_amount = int.Parse(line3);
            // Line 4: Total LTV value (USED TO WORK OUT THE MEAN)
            string line4 = sr.ReadLine(); int total_ltv = int.Parse(line4);
            // Storing all the values in an interger list
            List<int> stats = new List<int>();
            stats.Add(total_apps); stats.Add(successful_apps); stats.Add(total_loan_amount); stats.Add(total_ltv);
            sr.Close(); // stopping the stream reader
            return stats;
        }
        // Function to update the stats based on the most recent applicants
        static List<int> UpdateStats(List<int> current_stats, string file_path, bool loan_status, int ltv, int loan_val)
        {
            // setting all the stats to what they need to be for unsuccesful
            int total_apps = current_stats[0] + 1;
            int successful_apps = current_stats[1];
            int total_loan_val = current_stats[2];
            int total_ltv = current_stats[3] + ltv;
            // altering them further if the applicant was successful
            if (loan_status == true)
            {
                successful_apps += 1;
                total_loan_val += loan_val;
            }
            // storing the updated stats in a list so that they can be outputted to console
            List<int> new_stats = new List<int>();
            new_stats.Add(total_apps); new_stats.Add(successful_apps); new_stats.Add(total_loan_val); new_stats.Add(total_ltv);
            // creating a streamwriter to write the new data to the file
            StreamWriter sw = new StreamWriter(file_path, false);
            sw.WriteLine(total_apps); sw.WriteLine(successful_apps); sw.WriteLine(total_loan_val); sw.WriteLine(total_ltv);
            sw.Close();
            return new_stats;
        }
        // Function to output all of the loan data
        static void OutputData(List<int> stats, bool loan_status)
        {
            // Output status of the loan
            if (loan_status == true)
            {
                Console.WriteLine("Your loan application was successful! Congratulations");
                
            }
            else
            {
                Console.WriteLine("Your loan application was unfortunately denied.");
            }
            // Ouput the individual stats
            // applicants broke down by success
            Console.WriteLine("The Total Number of applicants so far is: ");
            Console.WriteLine(stats[0]);
            Console.WriteLine("The total number of successful applicants is: ");
            Console.WriteLine(stats[1]);
            Console.WriteLine("The total number of unsuccessful applicants is: ");
            int failed_apps = stats[0] - stats[1];
            Console.WriteLine(failed_apps);
            // total loan value
            Console.WriteLine("The total amount of all successful loans written to date is: ");
            Console.WriteLine(stats[2]);
            // mean ltv
            int mean_ltv = stats[3] / stats[0];
            Console.WriteLine("The mean LTV ratio for ALL applicants is: ");
            Console.WriteLine(mean_ltv);
           


        }
    }
}
