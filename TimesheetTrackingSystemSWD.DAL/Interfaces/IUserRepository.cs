using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimesheetTrackingSystemSWD.DAL.Models;


namespace TimesheetTrackingSystemSWD.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameAsync(string username); // @HoangDH
    }
}
