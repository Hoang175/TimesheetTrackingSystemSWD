using TimesheetTrackingSystemSWD.BLL.DTOs.AdEmployee;
using TimesheetTrackingSystemSWD.DAL.Models;


// @HoangDH
namespace TimesheetTrackingSystemSWD.BLL.Interfaces
{
    public interface IAdEmployeeService
    {
        (List<Employee> Data, int TotalPages) GetPagedEmployees(string? keyword, int? deptId, string? status, int page, int pageSize);

        bool CreateEmployee(AdEmployeeCreateDTO dto, out string message);

        AdEmployeeUpdateDTO? GetEmployeeForEdit(int id);
        bool UpdateEmployee(AdEmployeeUpdateDTO dto, out string message);

        bool InactiveEmployee(int id, out string message);
        bool DeleteEmployee(int id, out string message);

        bool ActiveEmployee(int id, out string message);
        bool RestoreEmployee(int id, out string message);
    }
}