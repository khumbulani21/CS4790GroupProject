
//checks hour block change in hours drop down
function HourBlockChange() {
    ProgramChange();
}
//checks if program has been changed in the program drop down
function ProgramChange() {
    let semesterID = document.getElementById("ddlSemester").value;
    Get(semesterID);
}

//gets enrollment count and enables and 
//disables submit buttons based on enrollment spots available
function Get(semesterID) {
    let progID = document.getElementById("ddlProgram").value;
     
    let hourBlock = document.getElementById("ddlHourBlock").value;
  
    if (semesterID != 0 && progID != 0) {
        let applicationID = document.getElementById("applicationID").value;

        $.ajax({
            type: 'GET',
            url: `/api/enrollmentCount?semesterID=${semesterID}&&programID=${progID}&&applicationID=${applicationID}`,
            dataType: 'json',
            success: function (data) {
                let display = data.count + " / " + data.programCapacity;
                document.getElementById("eCount").innerHTML = display;
                if (data.count < data.programCapacity && hourBlock != 0 && data.duplicateEnrollmentCount==0) {
                    document.getElementById("btnApprove").disabled = false;
                    
                }
                else {
                    
                    document.getElementById("btnApprove").disabled = true;
                    
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Error: ' + textStatus + ' - ' + errorThrown);
            }
        });
    }
    else {
        if (semesterID == 0 || progID == 0 ) {
           document.getElementById("eCount").innerHTML = "select semester and program to view enrollments";
            document.getElementById("btnApprove").disabled = true;
           
            

        }
        if (  hourBlock == 0) {
            document.getElementById("btnApprove").disabled = true;
          
        }
    }
}
//check if drop downs are empty
function ValidateInput() {

    //this one uses jquery to check if there is no id
    if ($('#ddlSemester option:selected').val() == 0) {
        swal('Error', 'Please Select a semester', 'error')
        return false;
    }
    if ($('#ddlProgram option:selected').val() == 0) {
        swal('Error', 'Please Select a program', 'error')
        return false;
    }
    if ($('#ddlHourBlock option:selected').val() == 0) {
        swal('Error', 'Please Select  hours', 'error')
        return false;
    }
    return true;
}