using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystemWeb.Models;
using OfficeOpenXml;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace TaskManagementSystemWeb.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskManagementContext _dbContext;

        public TaskController(TaskManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllTasks()
        {
            var tasks = _dbContext.TblTasks.ToList();
            return Ok(tasks);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetTaskById(int taskId)
        {
            var task = _dbContext.TblTasks.Find(taskId);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] TblTask task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                if (task.Id == 0) // zero means new task
                {
                    _dbContext.TblTasks.Add(task);
                    _dbContext.SaveChanges();
                    SendWelcomeEmail(task.EmailId, task.Assignee, task.Title, task.Description, task.DueDate.ToString("dd-MM-yyyy"));
                    return CreatedAtAction(nameof(GetTaskById), new { taskId = task.Id }, task);
                }
                else
                {
                    // ID means update task
                    var existingTask = _dbContext.TblTasks.Find(task.Id);
                    if (existingTask == null)
                    {
                        return NotFound();
                    }
                    existingTask.Title = task.Title;
                    existingTask.Description = task.Description;
                    existingTask.Assignee = task.Assignee;
                    existingTask.EmailId = task.EmailId;
                    existingTask.DueDate = task.DueDate;
                    existingTask.StatusId = task.StatusId; // Add this line to update the status

                    _dbContext.SaveChanges();
                    return Ok(existingTask);
                }
            }
        }

        [HttpDelete]
        public IActionResult DeleteTask(int taskId)
        {
            // Use DbSet<TblTask> to match the entity type
            var task = _dbContext.TblTasks.Find(taskId);

            if (task == null)
            {
                return NotFound();
            }

            _dbContext.TblTasks.Remove(task);
            _dbContext.SaveChanges();

            // Return NoContent for successful DELETE operations
            return NoContent();
        }

        
        public IActionResult DownloadTasksAsExcel()
        {
            var tasks = _dbContext.TblTasks.ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Tasks");

                // Add headers
                worksheet.Cells["A1"].Value = "Id";
                worksheet.Cells["B1"].Value = "Title";
                worksheet.Cells["C1"].Value = "Assignee";
                worksheet.Cells["D1"].Value = "Email ID";
                worksheet.Cells["E1"].Value = "Due Date";
                worksheet.Cells["F1"].Value = "Status";
                worksheet.Cells["G1"].Value = "Description";
                // Add other headers as needed

                // Populate data
                for (int i = 0; i < tasks.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = tasks[i].Id;
                    worksheet.Cells[i + 2, 2].Value = tasks[i].Title;
                    worksheet.Cells[i + 2, 3].Value = tasks[i].Assignee;

                    worksheet.Cells[i + 2, 4].Value = tasks[i].EmailId;
                    worksheet.Cells[i + 2, 5].Value = tasks[i].DueDate;
                    worksheet.Cells[i + 2, 6].Value = tasks[i].StatusId;
                    worksheet.Cells[i + 2, 7].Value = tasks[i].Description;
                    // Add other data as needed
                }
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TaskMgmReportData.xlsx");
            }
        }

        //send email of assigned task
        private void SendWelcomeEmail(string email, string assignname, string titile, string descrition, string duedate)
        {


            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); // Use port 465 for SSL
            client.EnableSsl = true;
            client.UseDefaultCredentials = false; // Ensure credentials are explicitly set
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential("ashfaque4070@gmail.com", "geix seid teny bwfs");

            MailMessage message = new MailMessage();
            message.From = new MailAddress("ashfaque4070@gmail.com");
            message.To.Add(email);
            message.Subject = " New Task Assigned Title :"+ titile;
            message.IsBodyHtml = true;
            message.Body = GetHTMLBOdy(assignname,titile,descrition,duedate);
            try
            {
                client.Send(message);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                // Log or handle the exception appropriately
            }

        }
        private string GetHTMLBOdy(string assignname, string titile, string descrition, string duedate)
        {
            string emailContent = @"
            <body>
                <div class='container' style='max-width: 100%px; margin: 20px auto; padding: 20px; background-color: #a0f3f8; border-radius: 7px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                    <h2 style='color: #333; margin-bottom: 20px;'>Task Manager - New Task Assigned - @TaskTitle</h2>
                    <b>Dear @AssigneeName,</b>
                    <p>I hope this email finds you well. We would like to inform you that a new task has been assigned to you in our Task Manager application. Here are the details of the task:</p>
        
                    <ul style='list-style: none; padding: 0; margin: 0;'>
                        <li><strong>Task Title:</strong> @TaskTitle</li>
                        <li><strong>Description:</strong> @TaskDescription</li>
                        <li><strong>Due Date:</strong> @DueDate</li>
                    </ul>
        
                    <p>Thank you for your attention to this matter, and we appreciate your dedication to completing tasks promptly.</p>
        
                    <p class='signature' style='margin-top: 20px;'>Best regards,<br>
                    Task Manager App Team</p>
                </div>
            </body>
            ";

            // Replace placeholders with actual values
            emailContent = emailContent.Replace("@AssigneeName", assignname);
            emailContent = emailContent.Replace("@TaskTitle", titile);
            emailContent = emailContent.Replace("@TaskDescription", descrition);
            emailContent = emailContent.Replace("@DueDate", duedate);

            // Use the emailContent variable as needed in your C# application

            return emailContent;
        }

    }
}
