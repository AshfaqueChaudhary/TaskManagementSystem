$(document).ready(function () {
    // Initialize Bootstrap's modal
    $('#confirmationModel').modal('hide');

    // Add the 'needs-validation' class to your form
    $('#taskForm').addClass('needs-validation');


    // Get tasks
    getTasks();
 
});


// Function to get tasks from the API
function getTasks() {
    $.ajax({
        url: 'https://localhost:7161/task/GetAllTasks',
        method: 'GET',
        success: function (tasks) {
            console.log('Received tasks:', tasks); 
            displayTasks(tasks);
        },
        error: function (error) {
            console.error('Error getting tasks:', error);
        }
    });
}


// Function to display tasks in the table
function displayTasks(tasks) {
    $('#taskList').empty();
    console.log('Email ID:', $('#emailid').val());
    tasks.forEach(function (task) {
        // Format the dueDate in DD/MM/YYYY
        const formattedDueDate = new Date(task.dueDate).toLocaleDateString('en-GB');
        $('#taskList').append(`
            <tr>
                <td>${task.id}</td>
                <td>${task.title}</td>
                <td>${task.assignee}</td>
                <td>${task.emailId}</td>
                <td>${formattedDueDate}</td>
                <td>${task.statusId}</td>
                <td>${task.description}</td>
                <td style="white-space: nowrap;">
                    <button id="editButton" class='btn btn-primary' onclick="editTask(${task.id})">Edit</button>
                    <button class='btn btn-primary' onclick="deleteTask(${task.id})">Delete</button>
                </td>
            </tr>
        `);
        
    });

}

// Function to add a new task
function addTask()
{
    console.log('Email ID:', $('#emailid').val());
    var taskData = {
        id: $('#taskId').val(),
        title: $('#title').val(),
        description: $('#description').val(),
        assignee: $('#assignee').val(),
        dueDate: $('#dueDate').val(),
        emailId: $('#emailid').val(),
        StatusId: $('#cmbStatus').val() // Default status
    };


    $.ajax({
        url: 'https://localhost:7161/task/CreateTask',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(taskData),
        success: function () {
            $("#btnTask").text("Add Task");
            $('#staticBackdrop').modal('show');
            
            getTasks(); // Refresh the task list after adding a new task
            // Clear the form
            debugger;
            console.log('task ID= '+('#taskId').val());
            if ($('#taskId').val() == 0)
            {
                /*$('#mdlNormalMessage').modal('show');*/
            }
            else {
                $('#staticBackdrop').modal('show');
                $('#taskForm')[0].reset();
            }
            
        },
        error: function (error) {
            console.error('Error adding task:', error);
        }
    });
}


function CancelPage() {
    location.reload(true);
}


// Function to populate the form controls for editing a task
function editTask(taskId)
{
    // Assuming there is a function to get a specific task by ID
    $.ajax({
        url: 'https://localhost:7161/task/GetTaskById?taskId='+taskId,
        method: 'GET',
        success: function (task)
        {
            
            // Populate the form controls with the task details
            $('#title').val(task.title);
            $('#description').val(task.description);
            $('#assignee').val(task.assignee);
            $('#emailid').val(task.emailId);
            // Assuming task.dueDate is "2024-01-12T00:00:00"
            var rawDate = task.dueDate.split('T')[0]; // Extracting the date part
            $('#dueDate').val(rawDate);

           // $('#dueDate').val(task.dueDate);
            $('#cmbStatus').val(task.statusId);
            // You can add more controls as needed

            // Store the taskId in a hidden input for reference
            $('#taskId').val(taskId);

            $("#btnTask").text("Update Task");

            // Change the text of the "Edit" button to "Update"
           // $('#editButton').text('Update');
        },
        error: function (error) {
            console.error('Error getting task details for editing:', error);
        }
    });
}

function deleteTask(taskId) {
    // Set the confirmation message in the modal
    $('#spanConfirmMsg').text("Are you sure you want to delete this task?");

    // Show the confirmation modal
    $('#confirmationModel').modal('show');
    
    $('#confirmButton').off('click').on('click', function () {
        // Close the modal
        $('#confirmationModel').modal('hide');

        // Proceed with the DELETE request
        $.ajax({
            
            url: 'https://localhost:7161/Task/DeleteTask?taskId=' + taskId,
            method: 'DELETE',
            success: function () {
                // Assuming you have a function to refresh the task list, e.g., getTasks()
                getTasks();
                console.log('Task deleted successfully.');
            },
            error: function (error) {
                console.error('Error deleting task:', error);
            }
        });
    });
}


// Function to download tasks in Excel format
function downloadExcel() {
    window.location.href = 'https://localhost:7161/task/DownloadTasksAsExcel';
}
