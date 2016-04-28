using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.IO;

namespace FileSystemWithTree
{

    public class DIR
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }

        public DIR (string id, string name, string parentId)
        {
            Id = id;
            Name = name;
            ParentId = parentId;
        }
    }


    public class Files
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PathToFile { get; set; }
        public string DIRId { get; set; } 

        public Files (string id, string name, string pathToFile, string dirId)
        {
            Id = id;
            Name = name;
            PathToFile = pathToFile;
            DIRId = dirId;
        }
    }

    public partial class _Default : Page
    {


        protected void TreeCreator(List<DIR> dirCollection, List<Files> filesCollection ,string parentId, TreeNodeCollection currentNode)
        {
            if (dirCollection.Count == 0)
                return;

            var dirCollectionInParent = from dir in dirCollection where dir.ParentId.Equals(parentId) select dir;

            var restOfCollection= from dir in dirCollection where dir.ParentId!=parentId select dir;

            foreach (var direct in dirCollectionInParent)
            {
                var nodeToAdd = new TreeNode(direct.Name, "directory "+direct.Name+" "+direct.Id+" "+direct.ParentId, "http://s32.postimg.org/uu0a91m81/dir.png");
                currentNode.Add(nodeToAdd);

                // filling added dir with files
                var filesInCurrentDIR = from files in filesCollection where files.DIRId.Equals(direct.Id) select files;
                foreach(var f in filesInCurrentDIR)
                {
                    var filenodeToAdd = new TreeNode(f.Name, "file " + f.Name + " " + f.Id + " " + f.DIRId+" "+f.PathToFile, "http://s32.postimg.org/dcyhr1i5t/file.png");
                    nodeToAdd.ChildNodes.Add(filenodeToAdd);
                }

                // next level of tree recursive execution
                TreeCreator(restOfCollection.ToList<DIR>(), filesCollection, direct.Id, nodeToAdd.ChildNodes);
            }

        }


        protected string PathToFile(string searchedNameOfFile ,string previousPath, TreeNodeCollection currentNode)
        {
            
        
            foreach (TreeNode tn in currentNode)
            {
                var strs = tn.Value.Split(' ');
                if (strs[1].Equals(searchedNameOfFile) && strs[0].Equals("file"))
                {
                    tn.Select();
                    return previousPath;

                }

               
                // next level of tree
                var res = PathToFile(searchedNameOfFile, previousPath + strs[1]+"\\", tn.ChildNodes);
                if (!res.Equals("there is no file with such name"))
                    return res;

            }

            return "there is no file with such name";
        
    }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<DIR> dirCollections = new List<DIR>();
                List<Files> filesCollections = new List<Files>();
                var cnn = new SqlConnection((string)Application["connectionString"]);

                try
                {

                    cnn.Open();
                    Label1.Text = "Connection Open ! ";
                    // creating dir colletions from db
                    using (SqlCommand dirReading = new SqlCommand("SELECT  * FROM DIR ", cnn))
                    {
                        //
                        // Instance methods can be used on the SqlCommand instance.
                        // ... These read data from executing the command.
                        //
                        using (SqlDataReader reader = dirReading.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // data mapping
                                dirCollections.Add(new DIR(reader.GetString(0), reader.GetString(1), reader.IsDBNull(2) ? "this is root" : reader.GetString(2)));

                            }
                        }
                    }

                    // creating files collection from db
                    using (SqlCommand filesReading = new SqlCommand("SELECT  * FROM Files_stored ", cnn))
                    {
                        //
                        // Instance methods can be used on the SqlCommand instance.
                        // ... These read data from executing the command.
                        //
                        using (SqlDataReader reader = filesReading.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // data mapping
                                filesCollections.Add(new Files(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));

                            }
                        }
                    }

                    cnn.Close();
                }
                catch (Exception ex)
                {
                    Label1.Text = ex.Message;
                }

                // creating dir tree
                if (dirCollections.Count != 0)
                {
                    //TreeView1.Nodes.Add(new TreeNode());
                    TreeCreator(dirCollections,filesCollections, "this is root", TreeView1.Nodes);
                }


            }
                
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var strs = TreeView1.SelectedNode.Value.Split(' ');
            var cnn = new SqlConnection((string)Application["connectionString"]);
            var physPath = (string)Application["PhysicalPath"];
            var newId = Guid.NewGuid().ToString();

            if (strs[0].Equals("directory"))
            {
                // uploading file
                
               
                string fname="";

                Files fileToUpload;
                //check file was submitted
                if (FileUpload1.HasFile)
                {
                    fname = Path.GetFileName(FileUpload1.FileName);
                    try
                    {
                        FileUpload1.SaveAs(Path.Combine(physPath, fname));
                    }
                   catch (Exception ex)
                    {
                        Label1.Text = "Check please if path You entered to save exist ";
                        return;
                    }
                
                    fileToUpload = new Files(newId, fname, physPath, strs[2]);
                }
                else
                    return;




                // making relation node in treee
               

                TreeView1.SelectedNode.ChildNodes.Add(new TreeNode(fileToUpload.Name, "file " + fileToUpload.Name + " " + fileToUpload.Id + " " + fileToUpload.DIRId, "http://s32.postimg.org/dcyhr1i5t/file.png"));

                // ading record to db
                try
                {

                    cnn.Open();

                    try
                    {
                        var queryFileAdding = String.Format("INSERT INTO Files_stored (Id,name,path_reference,dir_Id) VALUES ('{0}','{1}','{2}','{3}') ", fileToUpload.Id, fileToUpload.Name, fileToUpload.PathToFile,fileToUpload.DIRId);
                        SqlCommand dirAdding = new SqlCommand(queryFileAdding, cnn);
                        dirAdding.ExecuteReader();
                    }

                    catch (Exception ex)
                    {
                        Label1.Text = ex.Message;
                    }



                    cnn.Close();
                }
                catch (Exception ex)
                {
                    Label1.Text = ex.Message;
                }


            }

            else
                Label1.Text = "select folder";

        }

        

        protected void Button3_Click(object sender, EventArgs e)
        {
            string connectionString = null;
            
            connectionString = (string)Application["connectionString"];
            var cnn = new SqlConnection(connectionString);
            try
            {
                cnn.Open();
                Label1.Text= "Connection Open ! ";
                cnn.Close();
            }
            catch (Exception ex)
            {
                Label1.Text= ex.Message;
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            Application["connectionString"] = TextBox1.Text;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var strs = TreeView1.SelectedNode.Value.Split(' ');

            var cnn = new SqlConnection((string)Application["connectionString"]);


            if (strs[0].Equals("directory"))
            {
                var newId = Guid.NewGuid().ToString();
                // adding relation node to the tree
                TreeView1.SelectedNode.ChildNodes.Add(new TreeNode(TextBox1.Text, "directory " + TextBox1.Text+" "+ newId +" "+ strs[2], "http://s32.postimg.org/uu0a91m81/dir.png"));
                // adding reference to db
                try
                {

                    cnn.Open();

                    try
                    {
                        var queryDIRAdding = String.Format("INSERT INTO DIR (Id,name,parent_Id) VALUES ('{0}','{1}','{2}') ",newId,TextBox1.Text,strs[2]);
                        SqlCommand dirAdding = new SqlCommand(queryDIRAdding, cnn);
                        dirAdding.ExecuteReader();
                    }

                    catch (Exception ex)
                    {
                        Label1.Text = ex.Message;
                    }



                    cnn.Close();
                }
                catch (Exception ex)
                {
                    Label1.Text = ex.Message;
                }
            }

            else
                Label1.Text = "select folder";
        }

        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode node = new TreeNode();
            node = TreeView1.SelectedNode;
            Label1.Text = node.Value;
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            Application["PhysicalPath"] = TextBox2.Text;
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            Label1.Text = "";
            Label1.Text = PathToFile(TextBox1.Text,"",TreeView1.Nodes);
        }
    }
}