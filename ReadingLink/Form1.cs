using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CSALMongo;
using MongoDB.Bson;
using MongoDB.Driver;


namespace ReadingLink
{
    public partial class Form1 : Form
    {
        //should change the name of the database
        public const string DB_URL = "mongodb://localhost:27017/csaldata";
        //protected MongoDatabase testDB = null;

        public Form1()
        {
            InitializeComponent();

            // Start 
            try
            {
                //query from the database
                var db = new CSALDatabase(DB_URL);
                var students = db.FindStudents();
                var lessons = db.FindLessons();
                var classes = db.FindClasses();

                List<String> needStudentsLai = new List<string>();
                List<String> needStudentsMarietta = new List<string>();
                List<String> needStudentsAec = new List<string>();
                List<String> needStudentsKingw = new List<string>();
                List<String> needStudentsMain = new List<string>();
                List<String> needStudentsTlp = new List<string>();

                String readingUrlLai = "", readingUrlMarietta = "",
                    readingUrlAec = "", readingUrlKingw = "",
                    readingUrlMain = "", readingUrlTlp = "";

                // Generate all the tags
                //var Tags = "Record ID\tSubject ID\tClass ID\tLesson ID\tQuestion ID\tSection Level\tAttempt\tScore\tNumber of words in agent speeches\tTime spent on the item";
                //this.richTextBox1.Text = Tags;

                // find the target classes
                foreach (var oneClass in classes)
                {
                    //ace | kingwilliam | main | tlp
                    if (oneClass.ClassID == "aec")
                        //|| oneClass.ClassID == "kingwilliam" || oneClass.ClassID == "main" || oneClass.ClassID == "tlp"
                        needStudentsAec = addStudents(oneClass, needStudentsAec);

                    else if (oneClass.ClassID == "kingwilliam")
                        needStudentsKingw = addStudents(oneClass, needStudentsKingw);

                    else if (oneClass.ClassID == "main")
                        needStudentsMain = addStudents(oneClass, needStudentsMain);

                    else if (oneClass.ClassID == "tlp")
                        needStudentsTlp = addStudents(oneClass, needStudentsTlp);
                    // lai | marietta
                    else if (oneClass.ClassID == "lai")
                        //|| oneClass.ClassID == "marietta")
                        needStudentsLai = addStudents(oneClass, needStudentsLai);

                    else if (oneClass.ClassID == "marietta")
                        needStudentsMarietta = addStudents(oneClass, needStudentsMarietta);
                }

                //this.richTextBox1.Text = needStudentsA.Count.ToString();
                readingUrlAec = getReadingLink(needStudentsAec, db);
                readingUrlKingw = getReadingLink(needStudentsKingw, db);
                readingUrlLai = getReadingLink(needStudentsLai, db);
                readingUrlMain = getReadingLink(needStudentsMain, db);
                readingUrlMarietta = getReadingLink(needStudentsMarietta, db);
                readingUrlTlp = getReadingLink(needStudentsTlp, db);

                this.richTextBox1.Text = readingUrlTlp + "\n" + readingUrlMarietta + "\n"
                    + readingUrlMain + "\n" + readingUrlLai + "\n"
                    + readingUrlKingw + "\n" + readingUrlAec;

            }
            catch (Exception e)
            {
                e.GetBaseException();
                e.GetType();

            }

            
        }

        public List<String> addStudents(CSALMongo.Model.Class className, List<String> studentsList)
        {
            foreach (String student in className.Students)
            {
                if (!String.IsNullOrWhiteSpace(student) && !String.IsNullOrWhiteSpace(className.ClassID))
                {
                    String cS = className.ClassID + "-" + student;
                    studentsList.Add(cS);
                }
            }
            return studentsList;
        }

        public String getReadingLink(List<String> students, CSALDatabase db) {
            String readingUrl = "";
            foreach (String student in students) {
                String studentId = student.Split(new Char[] { '-' })[1];

                var studentLog = db.FindStudent(studentId);
                if (studentLog.ReadingURLs == null || studentLog.ReadingURLs.Count == 0)
                    continue;
                else {
                    foreach (var url in studentLog.ReadingURLs)
                    {
                        readingUrl += "\t" + url;
                    }
                }
                    
            }
            return readingUrl;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
