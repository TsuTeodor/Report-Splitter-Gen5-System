using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace SplitterGen5_SystemTest_v2
{
    public partial class Form1 : Form
    {

        OpenFileDialog FD = new OpenFileDialog();

        private string HeadOfHTML;
        private string BeforeTestCasePattern_extracted = "error_of_finding";
        private string testOverview = "error_of_finding";
        private int numberOfTestCasesExecuted = 0;
        private string htmlContent;
        private string result_EndOfTheFilePattern_match;
        private List<string> NameOfTestGroup = new List<string>();
        private List<int> IndexOfStartTestGroup = new List<int>();
        private List<int> IndexOfEndTestGroup = new List<int>();
        private List<int> ListOfTable = new List<int>();
        private int startIndexTestCase;
        //StreamWriter writer = new StreamWriter("C:/Data/Env/C#-enviroment/reports/report1.txt", true);
        private string htmlcontet_only_TC;
        private int MinimNumber;
        private int MaximNumber;
        private List<int> IndexOfTestCaseGroup = new List<int>();
        private List<int> IndexOfTestCaseGroupEnd = new List<int>();

        public string report_SW_Release;
        public string report_Target_HW_Version;
        public string report_Dataset;
        public string report_SW_Tester;
        public string report_Date;
        public List<string> report_Actual_Result = new List<string>();

        private MatchCollection startMatches_final = null;
        private MatchCollection endMatches_final = null;

        private List<int> AbsoluteNumberList_passed = new List<int>();
        private List<int> AbsoluteNumberList_failed = new List<int>();
        string filter_creation = "";
        private string filePath;
        public string pathHtmlFile = "";

        private string NoTcStarts = "";
        private string NoTcStarts_new = "";

        /* additional vars to get names and positions of tc*/
        Dictionary<string, int> testGroupStartData = new Dictionary<string, int>();
        Dictionary<string, int> endGroupStartData = new Dictionary<string, int>();
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        }

        public int Get_numberOfTestCasesExecuted()
        {
            return numberOfTestCasesExecuted; 
        }
        public void button1_Click(object sender, EventArgs e)
        {


            SplitReport();


            void SplitReport()
            {
                try
                {

                    if (NoTcStarts == "")
                    {

                        MessageBox.Show("No Deep level Number selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    // Path to HTML file
                    pathHtmlFile = HTML_input.Text.Replace(@"\\", @"/");

                    // Check if the source file exists
                    if (File.Exists(pathHtmlFile))
                    {
                        // Read the content of the HTML file
                        htmlContent = File.ReadAllText(pathHtmlFile);

                        // Extract the Head of HTML until Test Overview
                        ExtractHeadOfHTML();

                        // Extrat Test Overview from the Report
                        ExtratTestOverview();

                        // Extract the number of test cases executed from the Report
                        ExtratcNumberOfTestCasesExecuted();

                        // Split the test group from the Report
                        splitGroupSystem();

                        //splitGroup();
                        // Split the Test case from the Report
                        /*splitTestCase();

                        //Create filter with Absolute number for passed and failed
                        createFilterPassedFailed();

                        // Extract information that are needed for execution import
                        ExtractExecutionImport();

                        // Create CSV execution report that can be imported
                        CreateExecutionReport();
                        */

                    }
                    else
                    {
                        throw new FileNotFoundException("The source HTML file does not exist.");
                    }

                    MessageBox.Show("HTML split successfully ");
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            void degugInfo(string varbl)
            {
                if(varbl == "numberOfTestCasesExecuted")
                {
                    var noOfTests = Get_numberOfTestCasesExecuted();
                    String outInfo = noOfTests.ToString();
                    MessageBox.Show(outInfo);
                }
                else
                {
                    var noOfTests = Get_numberOfTestCasesExecuted();
                    String outInfo = noOfTests.ToString();
                    MessageBox.Show(outInfo);
                }
            }
            void ExtractHeadOfHTML()
            {
                try
                {
                    // Find the index of the start tag of Test Overview
                    int endStatisticsIndex = htmlContent.IndexOf("TestOverview\"></a>");

                    // Check if the target string was found
                    if (endStatisticsIndex != -1)
                    {
                        // Extract the content up to TestOverview\"></a>
                        string extractedContent = htmlContent.Substring(0, endStatisticsIndex);

                        // Write the extracted content to a string variable
                        HeadOfHTML = extractedContent;
                    }
                    else
                    {
                        throw new Exception("The target string TestOverview\"></a> was not found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            void ExtratTestOverview()
            {
                try
                {
                    // Find the index of the start tag of Test Overview
                    int startTestOverviewIndex = htmlContent.IndexOf(">Test Overview</div>");
                    int endTestOverviewIndex = htmlContent.IndexOf(">Test Module Information</div>");

                    // Check if the target string was found
                    if (startTestOverviewIndex != -1 && endTestOverviewIndex != -1)
                    {
                        // Extract the content after Test Overview
                        string extractedContent = htmlContent.Substring(startTestOverviewIndex, endTestOverviewIndex - startTestOverviewIndex);

                        // Write the extracted content to a string variable
                        testOverview = extractedContent;
                    }
                    else
                    {
                        throw new Exception("The target string Test Overview was not found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            /* void ExtratcNumberOfTestCasesExecuted()
             {
                 try
                 {
                     string pattern = "<td class=\"DefineCell\">Executed test cases </td>\\s*<td class=\"NumberCell\">(.*?)</td>";
                     Match match = Regex.Match(testOverview, pattern);

                     if (match.Success)
                     {
                         string value = match.Groups[1].Value;
                         numberOfTestCasesExecuted = Convert.ToInt16(value);
                         //Output.WriteLine("Number of test cases executed: " + numberOfTestCasesExecuted);
                     }
                     else
                     {
                         throw new Exception("Pattern for number of test cases executed not found in the string.");
                     }
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show(ex.Message);
                 }
             }
             */
            void ExtratcNumberOfTestCasesExecuted()
            {
                /*
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);*/

                var regexPattern = GetRegexPatternForDeepLevel();
                try
                {
                    Match match = Regex.Match(testOverview, regexPattern);
                    MatchCollection match2 = Regex.Matches(testOverview, regexPattern);
                    //if (match.Success)
                    if (match2.Count > 0)
                    {
                        /*
                        string value = match.Groups[1].Value;
                        numberOfTestCasesExecuted = Convert.ToInt16(value);
                        //Output.WriteLine("Number of test cases executed: " + numberOfTestCasesExecuted);
                        */
                        numberOfTestCasesExecuted = match2.Count;
                    }
                    else
                    {
                        throw new FileNotFoundException("Pattern for number of test cases executed not found in the string.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            void splitGroup()
            {
                try
                {
                    // Text to start copying from Test Case Details
                    string StartTestCaseSearch = "Test Case Details";

                    // Find the index where the search text appears
                    startIndexTestCase = htmlContent.IndexOf(StartTestCaseSearch);

                    if (startIndexTestCase == -1)
                    {
                        throw new Exception("Start test case search string not found in the HTML content.");
                    }

                    // Copy content starting from "Test Case Details" until the end
                    htmlcontet_only_TC = htmlContent.Substring(startIndexTestCase);

                    string NameOfTestGroup_unmodified;
                    string tableStartPattern = @"[0-9] Test Group: .*\n";
                    string tableEndPattern = @"<td.*?End of Test Group:.*?<\/td>";

                    // Find all matches for the beginning of test group
                    MatchCollection startMatches = Regex.Matches(htmlcontet_only_TC, tableStartPattern);

                    // Find all matches for the end of test group
                    MatchCollection endMatches = Regex.Matches(htmlcontet_only_TC, tableEndPattern);

                    if (startMatches.Count != endMatches.Count)
                    {
                        throw new Exception("Mismatch between start and end matches count.");
                    }

                    for (int count = 0; count < startMatches.Count; count++)
                    {
                        // Determine the start and end indices for the current section
                        int startIndex = startMatches[count].Index;
                        int endIndex = endMatches[count].Index + endMatches[count].Length;

                        int startIndexFinal = startMatches[count].Index + startMatches[count].Length;

                        // Remove charcaters "</a>" and new line from original string
                        NameOfTestGroup_unmodified = htmlcontet_only_TC.Substring(startIndex, startIndexFinal - startIndex).Replace("</a>", "").Replace("\n", "");

                        //Find the caracter "T" from TestGroup and remove what is before of that
                        int stopIndex = NameOfTestGroup_unmodified.IndexOf("T");

                        if (stopIndex == -1)
                        {
                            throw new Exception("Character 'T' not found in the test group name.");
                        }

                        // Add Test Group to the list
                        NameOfTestGroup.Add(NameOfTestGroup_unmodified.Substring(stopIndex));

                        // Add Index of the Start Test Group to the list
                        IndexOfStartTestGroup.Add(startIndex);

                        // Add Index of the End Test Group to the list
                        IndexOfEndTestGroup.Add(endIndex);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            void splitGroupSystem()
            {
                    /* "Copy content starting from "Test Case Details" until the end" moved into function GetTestGroupNames() */

                    GetTestGroupNames();
                    GetEndGroupPositions();


                    MessageBox.Show("No. of tests found: " + testGroupStartData.Count);

                    System.IO.Directory.CreateDirectory(html_output.Text + "//SplitDone");


                    if (testGroupStartData.Count == 0 || testGroupStartData.Count != endGroupStartData.Count) {
                        throw new FileNotFoundException("No. of test Groups is different than the no. of end groups");
                    }

                    /* create pattern
                     */
                    const string final_report_html_body = "\n    <table class=\"SubHeadingTable\">\n       <tr>\n        <td>\n          <div class=\"Heading2\">End of Report</div>\n        </td>\n      </tr>\n    </table>\n  </body>\n</html>";
                    //string html_till_TCase = htmlContent.Substring(0, startIndexTestCase);
                    string html_till_TCase = "";
                    string between_TCDetails = "<big class=\"Heading3\">";
                    int between_TCDetails_index = htmlcontet_only_TC.IndexOf(between_TCDetails);
                    between_TCDetails = htmlcontet_only_TC.Substring(0, between_TCDetails_index);
                    between_TCDetails += "<big class=\"Heading3\">";
                    //html_till_TCase += between_TCDetails;

                    foreach (string groupName in testGroupStartData.Keys)
                    {
                        if (!endGroupStartData.ContainsKey(groupName))
                        {
                            throw new Exception(groupName + " does not exists in endGroupStartData.");
                        }

                        /* adjust the text till tc */
                        html_till_TCase = GetTestTableResultForTest(groupName);
                        if(html_till_TCase == null)
                        {
                            throw new FileNotFoundException(groupName + " does not have a #i_ref no. in the html report therefore can't split the report.");
                    }
                        html_till_TCase += between_TCDetails;

                        /* <a name="i__400278672_9">1 Test Group: Cust_VBCVehicleInputIDCMStatSQCError</a> 
                         get the a name="" value*/
                        string i_value = GetTestGroupNameValue(htmlcontet_only_TC, groupName);
                    //string content_testCase = html_till_TCase + "\n            <a name=\"" + i_value + "\"> Test Group: ";
                        string content_testCase = html_till_TCase + "\n            <a name=\"" + i_value + "\"";
                        content_testCase += htmlcontet_only_TC.Substring(testGroupStartData[groupName], endGroupStartData[groupName] - testGroupStartData[groupName]);
                        string final_part_html_tc = "\n    <table class=\"GroupEndTable\">\n      <tr>\n        <td>End of Test Group: " + Regex.Escape(groupName) + "</td>\n      </tr>\n    </table>";
                        content_testCase += final_part_html_tc;
                        content_testCase += final_report_html_body;

                        /* change overall status of the test based on the content: if passed or failed */
                        content_testCase = ChangeStatusOfTestBasedOnContent(content_testCase);

                        /* save the testcase in html format */
                        CreateAndSaveTestGroup(content_testCase, groupName);
                    }

                /*}
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }*/
            }
/*
            void splitTestCase()
            {
                try
                {
                    matchStartIndexCase();

                    // Check if the number of starts and ends is equal
                    if (startMatches_final.Count != 0)
                    {
                        if (startMatches_final.Count == endMatches_final.Count)
                        {
                            // List to store the content of each <table> section
                            List<string> tablesContent = new List<string>();

                            //Output.WriteLine("startMatches.Count = " +startMatches.Count);
                            //Output.WriteLine("endMatches.Count = " +endMatches.Count);


                            // Iterate through each pair of start and end for the <table> section
                            for (int i = 0; i < startMatches_final.Count; i++)
                            {
                                // Determine the start and end indices for the current section
                                startIndexTestCase = startMatches_final[i].Index + startMatches_final[i].Length;
                                int endIndexTestCase = endMatches_final[i].Index + endMatches_final[i].Length;

                                // Extract the content between start and end
                                string tableContent = htmlcontet_only_TC.Substring(startIndexTestCase, endIndexTestCase - startIndexTestCase);

                                //Output.WriteLine("startIndexTestCase " + i + " = " + startIndexTestCase);
                                //Output.WriteLine("endIndexTestCase " + i + " = " + endIndexTestCase);

                                tableContent = "\n    <table>\n      <tbody><tr> \n        <td class=\"LinkCell\">" + tableContent;

                                // Add the content to the list of <table> sections
                                tablesContent.Add(tableContent);

                                *//* inside bellow IF the code parse thru the indexes of the START GROUP and END GROUP 
                                the parsing is done with first element of the START GROUP with each elements from END GROUP
                                the parsing is done with second element of the START GROUP with each elements from END GROUP
                                .... and so on, until is done with all GROUP
                                after that, the comparison is done with evey TC index and START GROUP and END GROUP
                                store the maximum value of the START GROUP and minimum value of the END GROUP*//*
                    if (IndexOfStartTestGroup.Count == IndexOfEndTestGroup.Count)
                                {
                                    MinimNumber = int.MaxValue;
                                    MaximNumber = int.MinValue;
                                    // verify the Test Group for each test case
                                    for (int countStart = 0; countStart < IndexOfStartTestGroup.Count; countStart++)
                                    {

                                        for (int countEnd = 0; countEnd < IndexOfEndTestGroup.Count; countEnd++)
                                        {
                                            if (startIndexTestCase > IndexOfStartTestGroup[countStart] && startIndexTestCase < IndexOfEndTestGroup[countEnd])
                                            {
                                                //ListOfTable.Add(1);
                                                if (MaximNumber < IndexOfStartTestGroup[countStart])
                                                {
                                                    MaximNumber = IndexOfStartTestGroup[countStart];
                                                    MinimNumber = int.MaxValue;
                                                }
                                                if (MinimNumber > IndexOfEndTestGroup[countEnd])
                                                {
                                                    MinimNumber = IndexOfEndTestGroup[countEnd];
                                                }

                                                //writer.WriteLine("startIndexTestCase = " + startIndexTestCase + " listoftabe = " + "1 " + " IndexOfStartTestGroup[countStart] = " + IndexOfStartTestGroup[countStart] + " IndexOfEndTestGroup[countEnd] = " + IndexOfEndTestGroup[countEnd] + " MaximNumber = " + MaximNumber + " MinimNumber = " + MinimNumber);
                                            }
                                            else
                                            {
                                                //ListOfTable.Add(0);
                                                //writer.WriteLine("startIndexTestCase = " + startIndexTestCase + " listoftabe = " + "0 " + " IndexOfStartTestGroup[countStart] = " + IndexOfStartTestGroup[countStart] + " IndexOfEndTestGroup[countEnd] = " + IndexOfEndTestGroup[countEnd] );
                                            }
                                        }
                                    }
                                    IndexOfTestCaseGroup.Add(MaximNumber);
                                    IndexOfTestCaseGroupEnd.Add(MinimNumber);

                                }
                                else
                                {
                                    throw new Exception("Number of TestGroup start != Number of TestGroup end");
                                }

                            }
                            //writer.Close();

                            int nr_test = 0;
                            // Create the reports for each of testcase found (tableContent)
                            foreach (var tableContent in tablesContent)
                            {
                                // Regular expression for extracting the value of testcase
                                string TestCaseName = @"Test Case (\w+):";

                                // Create a string variable for naming the reports files
                                string result_testCaseName = "error_naming_file";

                                Match TestCaseName_match = Regex.Match(tableContent, TestCaseName);

                                if (TestCaseName_match.Success)
                                {
                                    // store the string after Test Case
                                    result_testCaseName = TestCaseName_match.Groups[1].Value;
                                }

                                // Regular expression to find and extract the value after "Report:"
                                string ReportPattern = @"<big class=""Heading1"">Report: (\w+)</big>";

                                // Check if the string contains the pattern and extract the value
                                Match ReportPattern_match = Regex.Match(HeadOfHTML, ReportPattern);

                                string HeadOfHTML_modified = "error";

                                if (ReportPattern_match.Success)
                                {
                                    // Extract the value after "Report:"
                                    string extractedValue = ReportPattern_match.Groups[1].Value;

                                    // Replace the Report string with with "Report" + test case name
                                    HeadOfHTML_modified = HeadOfHTML.Replace(ReportPattern_match.Value, "<big class=\"Heading1\">" + "Report: " + result_testCaseName + "</big>");

                                    // Regular expression to find the string between <title> </title>
                                    string titlePattern = "<title>[^<]*</title>";


                                    // Replace the title name with test case name
                                    HeadOfHTML_modified = Regex.Replace(HeadOfHTML_modified, titlePattern, "<title>" + result_testCaseName + "</title>");

                                }
                                else
                                {
                                    throw new Exception("Pattern for Report name not found in the string.");
                                }

                                string passed_failed_folder = "error";
                                // verify if the report of the testcase is Failed, then replace the OverallResult with Failed. And also if the report of the testcase is Passed, then replace the  OverallResult with Passed
                                if (tableContent.Contains("<td class=\"TestcaseHeadingPositiveResult\">"))
                                {
                                    HeadOfHTML_modified = HeadOfHTML_modified.Replace("<td class=\"NegativeResult\">Test failed</td>", "<td class=\"PositiveResult\">Test passed</td>");

                                    // set folder where report will be saved with Passed
                                    passed_failed_folder = "/Passed/";

                                    // put in the passed list the absolute number of the test case
                                    filter_creation = System.Text.RegularExpressions.Regex.Replace(result_testCaseName, "[^0-9.]", "");
                                    AbsoluteNumberList_passed.Add(Convert.ToInt16(filter_creation));
                                }
                                else
                                {
                                    HeadOfHTML_modified = HeadOfHTML_modified.Replace("<td class=\"PositiveResult\">Test passed</td>", "<td class=\"NegativeResult\">Test failed</td>");

                                    // set folder where report will be saved with Failed
                                    passed_failed_folder = "/Failed/";
                                    // put in the failed list the absolute number of the test case
                                    filter_creation = System.Text.RegularExpressions.Regex.Replace(result_testCaseName, "[^0-9.]", "");
                                    AbsoluteNumberList_failed.Add(Convert.ToInt16(filter_creation));
                                }

                                string name_TestGroup = "Error, no Test Group was found";
                                string name_TestGroupEnd = "Error, no Test Group was found";
                                string TestGroupOnly = "Error, no Test Group was found";

                                for (int count = 0; count < IndexOfStartTestGroup.Count; count++)
                                {
                                    if (IndexOfTestCaseGroup[nr_test] == IndexOfStartTestGroup[count])
                                    {
                                        // put in name_TestGroup the tags before Test Group
                                        name_TestGroup = "<div id=\"div_1\">\n <div class=\"Indentation\"></div>\n </div>\n <table class=\"GroupHeadingTable\">\n <tbody><tr>\n <td class=\"LinkCell\">\n <a class=\"Undecorated\"" +
                                        "id=\"lnk_1.1\" href=\"javascript:switchAll(&#39;1.1&#39;,document.all[&#39;lnk_1.1&#39;].text)\">[−]</a>\n </td>\n <td>\n <big class=\"Heading3\">\n <a name=\"i__1133176832_85\">";

                                        TestGroupOnly = NameOfTestGroup[count];

                                        name_TestGroup = name_TestGroup + TestGroupOnly;

                                        // put in name_TestGroup the tags after Test Group
                                        name_TestGroup = name_TestGroup + "</a>\n </big>\n </td>\n </tr>\n </tbody></table>\n <div id=\"div_1.1\">\n <div class=\"Indentation\"></div>\n </div>\n <table>\n <tbody><tr>\n";
                                    }
                                }

                                // put in name_TestGroupEnd the tags before and after End of Test Group
                                name_TestGroupEnd = "<table class=\"GroupEndTable\">\n <tbody><tr>\n <td>" + "End of " + TestGroupOnly + "</td>\n </tr>\n </tbody></table>\n <table class=\"GroupHeadingTable\">\n";


                                System.IO.Directory.CreateDirectory(html_output.Text + "//Passed");
                                System.IO.Directory.CreateDirectory(html_output.Text + "//Failed");


                                File.WriteAllText(html_output.Text + "" + passed_failed_folder + result_testCaseName + ".html", HeadOfHTML_modified + name_TestGroup + tableContent + name_TestGroupEnd);

                                nr_test++;
                            }
                        }
                        else
                        {
                            throw new Exception("Number of Testcase start != Number of Testcase end");
                        }
                    }
                    else
                    {
                        throw new Exception("Mismatch between the number of <table> start tags and </table> end tags.");
                    }
                }
                catch (DirectoryNotFoundException dirEx)
                {
                    // Handle Directory Not Found Exception if directory was not found
                    MessageBox.Show("Directory not found: " + dirEx.Message);
                }
                catch (IOException ioEx)
                {
                    // Handle IO Exception if any IO operation fails
                    MessageBox.Show("An IO error occurred: " + ioEx.Message);
                }
                catch (Exception ex)
                {
                    // Handle any other exception
                    MessageBox.Show("An error occurred: " + ex.Message);
                }

            }

            void createFilterPassedFailed()
            {
                string filter = "";
                bool first = true;

                if (AbsoluteNumberList_failed.Count() == 1)
                {
                    filter = "(Absolute Number == " + AbsoluteNumberList_failed[0] + ") ";
                }
                else if (AbsoluteNumberList_failed.Count() == 0)
                {
                    filter = "No failed TC ";
                }
                else
                {
                    for (int count = 0; count < AbsoluteNumberList_failed.Count() - 1; count++)
                    {
                        filter += "(";
                    }

                    foreach (int number in AbsoluteNumberList_failed)
                    {
                        if (first == true)
                        {
                            filter += "Absolute Number == " + number + ")";
                            first = false;
                        }
                        else
                        {
                            filter += " OR (Absolute Number == " + number + "))";
                        }
                    }
                }
                if (filter.Length > 0)
                {
                    filter = filter.Remove(filter.Length - 1);
                }

                filePath = System.IO.Path.Combine(html_output.Text, "failed_doors_filter.txt");
                File.WriteAllText(filePath, filter);

                filter = "";
                first = true;
                for (int count = 0; count < AbsoluteNumberList_passed.Count() - 1; count++)
                {
                    filter += "(";
                }

                if (AbsoluteNumberList_passed.Count() == 1)
                {
                    filter = "(Absolute Number == " + AbsoluteNumberList_passed[0] + ") ";
                }
                else if (AbsoluteNumberList_passed.Count() == 0)
                {
                    filter = "No failed TC ";
                }
                else
                {
                    foreach (int number in AbsoluteNumberList_passed)
                    {
                        if (first == true)
                        {
                            filter += "Absolute Number == " + number + ")";
                            first = false;
                        }
                        else
                        {
                            filter += " OR (Absolute Number == " + number + "))";
                        }
                    }
                }

                if (filter.Length > 0)
                {
                    filter = filter.Remove(filter.Length - 1);
                }

                filePath = System.IO.Path.Combine(html_output.Text, "passed_doors_filter.txt");
                File.WriteAllText(filePath, filter);

            }

            void ExtractExecutionImport()
            {
                try
                {
                    string pattern = "SW Release: </td>\\s*<td class=\"CellNoColor\">(.*?)</td>";
                    var match = System.Text.RegularExpressions.Regex.Match(HeadOfHTML, pattern);

                    if (match.Success)
                    {
                        report_SW_Release = match.Groups[1].Value;
                    }
                    else
                    {
                        throw new Exception("SW Release - was not found");
                    }

                    pattern = @"Target HW Version: </td>\s*<td class=""CellNoColor"">(.*?)</td>";
                    match = System.Text.RegularExpressions.Regex.Match(HeadOfHTML, pattern);

                    if (match.Success)
                    {
                        report_Target_HW_Version = match.Groups[1].Value;
                    }
                    else
                    {
                        throw new Exception("Target HW Version - was not found");
                    }

                    pattern = @"Dataset: </td>\s*<td class=""CellNoColor"">(.*?)</td>";
                    match = System.Text.RegularExpressions.Regex.Match(HeadOfHTML, pattern);

                    if (match.Success)
                    {
                        report_Dataset = match.Groups[1].Value;
                    }
                    else
                    {
                        throw new Exception("Dataset - was not found");
                    }

                    pattern = @"SW Tester: </td>\s*<td class=""CellNoColor"">(.*?)</td>";
                    match = System.Text.RegularExpressions.Regex.Match(HeadOfHTML, pattern);

                    if (match.Success)
                    {
                        report_SW_Tester = match.Groups[1].Value;
                    }
                    else
                    {
                        throw new Exception("SW Tester - was not found");
                    }

                    pattern = @"Test begin: </td>\s*<td style=""padding-right: 0.5em;"" class=""CellNoColor"">(.*?) ";
                    match = System.Text.RegularExpressions.Regex.Match(testOverview, pattern);

                    if (match.Success)
                    {
                        report_Date = match.Groups[1].Value;
                    }
                    else
                    {
                        throw new Exception("Date(Test Begin) - was not found");
                    }
                }
                catch (Exception ex)
                {
                    // Handle or log the exception
                    MessageBox.Show(ex.Message);
                }
            }
*/

            void CreateExecutionReport()
            {
                if (AbsoluteNumberList_failed.Count() >= 1)
                {
                    filePath = System.IO.Path.Combine(html_output.Text + "/failed_doors_execution.csv");
                    File.WriteAllText(filePath, "Absolute Number,TP_TestPlanned_Rel_ " + NoTcStarts + "," + "TP_ActualResult_Rel_ " + NoTcStarts + "," + "TP_PTC_Ref_Rel_ " + NoTcStarts + "," + "TP_RespTester_Rel_ " + NoTcStarts + "," + "TP_TestDate_Rel_ " + NoTcStarts + "," + "TP_TestObject_Rel_ " + NoTcStarts + "," + "TP_TestVerdict_Rel_ " + NoTcStarts + "," + "\n");

                    foreach (int number in AbsoluteNumberList_failed)
                    {
                        File.AppendAllText(filePath, number + "," + "Planned," + "see test report," + "Fill with CR number," + report_SW_Tester + "," + report_Date + ",\"" + report_SW_Release + "\n" +
                        report_Target_HW_Version + "\n" + report_Dataset + "\"," + "Failed" + "\n");
                    }
                }
                else
                {
                    // do nothing
                }

                if (AbsoluteNumberList_passed.Count() >= 1)
                {
                    filePath = System.IO.Path.Combine(html_output.Text + "/passed_doors_execution.csv");
                    File.WriteAllText(filePath, "Absolute Number,TP_TestPlanned_Rel_ " + NoTcStarts + "," + "TP_ActualResult_Rel_ " + NoTcStarts + "," + "TP_PTC_Ref_Rel_ " + NoTcStarts + "," + "TP_RespTester_Rel_ " + NoTcStarts + "," + "TP_TestDate_Rel_ " + NoTcStarts + "," + "TP_TestObject_Rel_ " + NoTcStarts + "," + "TP_TestVerdict_Rel_ " + NoTcStarts + "," + "\n");

                    foreach (int number in AbsoluteNumberList_passed)
                    {
                        File.AppendAllText(filePath, number + "," + "Planned," + "see test report," + "NA," + report_SW_Tester + "," + report_Date + ",\"" + report_SW_Release + "\n" +
                        report_Target_HW_Version + "\n" + report_Dataset + "\"," + "Passed" + "\n");
                    }
                }
                else
                {
                    // do nothing
                }
            }

            void matchStartIndexCase()
            {
                try
                {
                    bool found = false;

                    // Define regular expressions to identify <table> sections (test cases information)
                    string tableStartPattern = "<table>\\s+<tbody><tr>\\s+<td class=\"LinkCell\">";
                    string tableEndPattern = "\\s*</tr>\\s*</tbody></table>\\s*</div>\\s*</div>";

                    // Find all matches for the beginning of <table> sections
                    MatchCollection startMatches = Regex.Matches(htmlcontet_only_TC, tableStartPattern);

                    // Find all matches for the end of </table> sections
                    MatchCollection endMatches = Regex.Matches(htmlcontet_only_TC, tableEndPattern);

                    if ((startMatches.Count == endMatches.Count) && (startMatches.Count == numberOfTestCasesExecuted))
                    {
                        startMatches_final = startMatches;
                        endMatches_final = endMatches;
                        found = true;
                    }

                    if (false == found)
                    {
                        // Define regular expressions to identify <table> sections (test cases information)
                        tableStartPattern = "<table>\\s+<tbody><tr>\\s+<td class=\"LinkCell\">";
                        tableEndPattern = "</tr>\\s+</tbody></table>\n\\s+</div>\n\\s+</div>";

                        // Find all matches for the beginning of <table> sections
                        startMatches = Regex.Matches(htmlcontet_only_TC, tableStartPattern);

                        // Find all matches for the end of </table> sections
                        endMatches = Regex.Matches(htmlcontet_only_TC, tableEndPattern);
                        if ((startMatches.Count == endMatches.Count) && (startMatches.Count == numberOfTestCasesExecuted))
                        {
                            startMatches_final = startMatches;
                            endMatches_final = endMatches;
                            found = true;
                        }
                    }
                    if (false == found)
                    {
                        // Define regular expressions to identify <table> sections (test cases information)
                        tableStartPattern = "<table>\\s*<tr>\\s*<td class=\"LinkCell\">";
                        tableEndPattern = "</tr>\\s*</table>\\s*</div>\\s*</div>";

                        // Find all matches for the beginning of <table> sections
                        startMatches = Regex.Matches(htmlcontet_only_TC, tableStartPattern);

                        // Find all matches for the end of </table> sections
                        endMatches = Regex.Matches(htmlcontet_only_TC, tableEndPattern);
                        if ((startMatches.Count == endMatches.Count) && (startMatches.Count == numberOfTestCasesExecuted))
                        {
                            startMatches_final = startMatches;
                            endMatches_final = endMatches;
                            found = true;
                        }

                    }
                    if (false == found)
                    {
                        throw new Exception("Unable to find matching start and end patterns for Test Cases in the HTML content.");
                    }




                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            string GetRegexPatternForDeepLevel()
            {
                var deep_level = 0;
                try
                {
                    deep_level = Int32.Parse(NoTcStarts);
                    
                    if (deep_level < 0)
                    {
                        MessageBox.Show("The no. provided has to be a positive integer");
                        throw new Exception();
                    }
                }
                catch (Exception exep)
                {
                    MessageBox.Show("The no. provided as NoTcStarts is not valid. Introduce a new value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                string numberPattern = "[0-9]+";
                for (int i = 0; i < deep_level; i++)
                {
                    numberPattern += @"\.[0-9]+";
                }

                string pattern = @"<tr>\s*<td class=""DefineCell"">\s*<b>";
                pattern += numberPattern;
                pattern += "</b>";
                return pattern;
            }
            string GetRegexPatternForDeepLevelTestGroupStart()
            {
                var deep_level = 0;
                try
                {
                    deep_level = Int32.Parse(NoTcStarts);

                    if (deep_level < 0)
                    {
                        MessageBox.Show("The no. provided has to be a positive integer");
                        throw new Exception();
                    }
                }
                catch (Exception exep)
                {
                    MessageBox.Show("The no. provided as NoTcStarts is not valid. Introduce a new value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                string numberPattern = "[0-9]+";
                for (int i = 0; i < deep_level; i++)
                {
                    numberPattern += @"\.[0-9]+";
                }

                string pattern = ">";
                pattern += numberPattern;
                pattern += @" Test Group: ";
                return pattern;
            }
            void GetTestGroupNames()
            {/*
                try
                {*/
                    // Text to start copying from Test Case Details
                    string StartTestCaseSearch = "Test Case Details";

                    // Find the index where the search text appears
                    startIndexTestCase = htmlContent.IndexOf(StartTestCaseSearch);

                    if (startIndexTestCase == -1)
                    {
                        throw new FileNotFoundException("Start test case search string not found in the HTML content.");
                    }

                    // Copy content starting from "Test Case Details" until the end
                    htmlcontet_only_TC = htmlContent.Substring(startIndexTestCase);

                    //string pattern = GetRegexPatternForDeepLevelTestGroupStart() + @" (.*?)<\/a>";
                    string pattern = GetRegexPatternForDeepLevelTestGroupStart() + @"([^<]+)<\/a>";
                    //string pattern = GetRegexPatternForDeepLevelTestGroupStart();
                    Regex regex = new Regex(pattern, RegexOptions.Singleline);
                    MatchCollection matches = regex.Matches(htmlcontet_only_TC);

                    testGroupStartData = new Dictionary<string, int>();
                    foreach (Match match in matches)
                    {
                        string groupName = match.Groups[1].Value;
                        int positionGroup = match.Index;
                        if (testGroupStartData.ContainsKey(groupName))
                        {
                            throw new FileNotFoundException("Can't have multiple tests with the same name.\nChange the NoTcStarts.");
                        }
                        testGroupStartData.Add(groupName, positionGroup);
                    }

                /*}
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }*/
            }
            void GetEndGroupPositions()
            {/*
                try
                {*/
                endGroupStartData = new Dictionary<string, int>();
                    foreach (string groupName in testGroupStartData.Keys)
                    {
                        string endGroupPattern = $@"<table class=""GroupEndTable"">\s*<tr>\s*<td>End of Test Group: {Regex.Escape(groupName)}</td>\s*</tr>\s*</table>";
                        Match endGroupMatch = Regex.Match(htmlcontet_only_TC, endGroupPattern, RegexOptions.Singleline);

                        if (endGroupMatch.Success)
                        {
                            int endGroupPosition = endGroupMatch.Index;
                            if (endGroupStartData.ContainsKey(groupName))
                            {
                                throw new FileNotFoundException("Can't have multiple tests with the same name.\nChange the NoTcStarts.");
                            }
                            endGroupStartData.Add(groupName, endGroupPosition);
                        }
                    }
                    foreach (var entry in endGroupStartData)
                    {
                        Console.WriteLine($"Group: {entry.Key} ends at position {entry.Value}");
                    }
                /*}
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }*/
            }
            string GetTestGroupNameValue(string htmlContent, string testGroupName)
            {
                string pattern = $@"<a name=""([^""]+)"">\d+(\.\d+)*\s*Test Group:\s*{Regex.Escape(testGroupName)}</a>";
                Regex regex = new Regex(pattern, RegexOptions.Singleline);
                Match match = regex.Match(htmlContent);

                if (match.Success)
                {
                    return match.Groups[1].Value;
                }

               
                return null;
            }
            void CreateAndSaveTestGroup(string content, string testGroupName)
            {
                try
                {


                    //System.IO.Directory.CreateDirectory(html_output.Text + "//Passed");
                    //System.IO.Directory.CreateDirectory(html_output.Text + "//Failed");

                    int realative_index = 1;
                    foreach(var item in testGroupStartData.Keys)
                    {
                        if (item == testGroupName)
                        {
                            break;
                        }
                        else
                        {
                            realative_index++;
                        }
                    }

                    File.WriteAllText(html_output.Text + "/SplitDone/" + realative_index + ".html", content);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in function CreateAndSaveTestGroup");
                }
            }
            string GetTestTableResultForTest(string testGroupName)
            {

                string html_after_results = "<div class=\"Heading4\">Test Case Results</div>";
                int html_after_results_index = htmlContent.IndexOf(html_after_results);

                /* substract the html content till start of table*/
                const string start_of_table = "<td class=\"DefineCell\" colspan=\"2\">";
                int index_start_of_table = htmlContent.IndexOf(start_of_table);


                /* getting out the statistics table*/
                string statistics_table = "<div class=\"Heading4\">Statistics</div>";
                int statistics_table_index = htmlContent.IndexOf(statistics_table);
                string content_html_till_table = "";

                //string content_html_till_table = htmlContent.Substring(0, index_start_of_table);

                //only if there is a statistics table inside the report
                if (statistics_table_index > 0)
                {
                    content_html_till_table = htmlContent.Substring(0, statistics_table_index);
                    string between_TC_details_and_actually_result = htmlContent.Substring(html_after_results_index, index_start_of_table - html_after_results_index);
                    content_html_till_table += between_TC_details_and_actually_result;
                }
                else
                {
                    content_html_till_table = htmlContent.Substring(0, index_start_of_table);
                }
                /* substract the relevant table content */
                var i_value_current_tc = GetTestGroupNameValue(htmlContent, testGroupName);
                if(i_value_current_tc == null)
                {
                    throw new FileNotFoundException(testGroupName + " does not have a #i_ref no. in the html report therefore can't split the report.");
                    //return null;
                }
                string relative_next_tcName = "";
                Boolean found = false;
                foreach( var item in testGroupStartData.Keys)
                {
                    if (item == testGroupName)
                    {
                        found = true;
                        continue;
                    }
                    if(found == true)
                    {
                        relative_next_tcName = item;
                        break;
                    }
                }
                if(relative_next_tcName == "")
                {
                    found = false;
                }
                string start_content_table = "<a href=\"#" + i_value_current_tc + "\">";
                string stop_content_table = "";
                int index_start_in_table_this_tc = htmlContent.IndexOf(start_content_table);
                string substr_content_table = "";

                string testModuleInfo = "<a name=\"TestModuleInfo\"></a>";
                int testModuleInfo_index = htmlContent.IndexOf(testModuleInfo);
                /* calculate where to stop in table content */
                if (!found)
                {
                    //<a name="TestModuleInfo"></a>
                    
                    substr_content_table = htmlContent.Substring(index_start_in_table_this_tc, testModuleInfo_index - index_start_in_table_this_tc);

                }
                else 
                {
                    var i_value_next_tx = GetTestGroupNameValue(htmlContent, relative_next_tcName);
                    stop_content_table = "<a href=\"#" + i_value_next_tx + "\">";
                    int index_stop = htmlContent.IndexOf(stop_content_table);
                    substr_content_table = htmlContent.Substring(index_start_in_table_this_tc, index_stop - index_start_in_table_this_tc);

                    /* because we used as reference a following testcase, 
                     * we need to substract to reach the proper ending of the current one */
                    const string recalulcate_till = "</tr>";
                    int recalulcate_till_index = substr_content_table.LastIndexOf(recalulcate_till);
                    substr_content_table = substr_content_table.Substring(0, recalulcate_till_index);
                    substr_content_table += recalulcate_till;

                    /* the content is delimited now
                     */

                    substr_content_table += "</table>      </ div >    </ div >";
                }

                /* add html code between TestModuleInfo to TestCaseDetails*/
                string testCaseDetails = "Test Case Details</big>";
                int testcaseDetails_index = htmlContent.IndexOf(testCaseDetails);
                substr_content_table += htmlContent.Substring(testModuleInfo_index, testcaseDetails_index - testModuleInfo_index);
                string combined_results = content_html_till_table + substr_content_table;
                return combined_results;
            }
            Boolean GetStatusOfTestBasedOnContent(string content)
            {
                Boolean result = true;
                string pattern_to_find_overall_status = @"class=""NegativeResultCell"">";
                int pattern_to_find_overall_status_index = content.IndexOf(pattern_to_find_overall_status);
                if(pattern_to_find_overall_status_index > 0)
                {
                    result = false;
                }
                return result;
            }
            string ChangeStatusOfTestBasedOnContent(string content)
            {
                string replacement = "";
                string pattern = "<td class=\"NegativeResult\">Test failed</td>";

                if (GetStatusOfTestBasedOnContent(content))
                {
                    /* Positive result */
                    replacement = "<td class=\"PositiveResult\">Test passed</td>";
                }
                else
                {
                    /* Negative result - No need to change since it is the default case */
                    return content;
                }
                content = Regex.Replace(content, pattern, replacement);
                return content;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (FD.ShowDialog() == DialogResult.OK)
            {

                HTML_input.Text = Path.GetFullPath(FD.FileName);
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderdiag = new FolderBrowserDialog();
            if (folderdiag.ShowDialog() == DialogResult.OK)
            {

                html_output.Text = folderdiag.SelectedPath;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            const string firstLine = "NoTcStarts represents the no. of dots (.) from where a test starts";
            const string secondLine = "Your report may include no header, one header or more.";
            const string thirdLine = "\n\n\n";
            const string fourthLine = "If you receive the error with \"Can't have multiple tests with the same name..\"";
            const string fivethLine = "it means that you're trying to break the report into some identical reports.";
            const string sixthLine = "\nFirst use a lower {NoTcStarts} to separate the report into individual testcases and";
            const string seventhLine = "rerun the app with the resulted testcase till you obtain the expected result";
            MessageBox.Show(firstLine + secondLine + thirdLine + fourthLine + fivethLine + sixthLine + seventhLine, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void HTML_input_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {


            NoTcStarts_new = textBox_NoTcStarts.Text;

            if (NoTcStarts_new == "")
            {
                MessageBox.Show("No Rel Number selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            NoTcStarts = NoTcStarts_new;
            NoTcStarts_Label.Text = "NoTcStarts = " + textBox_NoTcStarts.Text;
            NoTcStarts_Label.Visible = true;
        }
    }
}
