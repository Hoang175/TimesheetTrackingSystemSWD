using TimesheetTrackingSystemSWD.BLL.DTOs.AdDepartment;

namespace TimesheetTrackingSystemSWD.BLL.Interfaces
{
    public interface IAdDepartmentService
    {
        List<AdDepartmentDTO> GetAllDepartments(string? keyword, bool isDeletedView = false); 
        bool RestoreDepartment(int id, out string message); 
        AdDepartmentDTO? GetDepartmentForEdit(int id);
        bool CreateDepartment(AdDepartmentDTO dto, out string message);
        bool UpdateDepartment(AdDepartmentDTO dto, out string message);
        bool DeleteDepartment(int id, out string message);
    }
}