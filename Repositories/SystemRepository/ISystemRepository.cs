using System.Collections.ObjectModel;

using Gym_Reception_Management_System.Models;

namespace Gym_Reception_Management_System.Repositories.SystemRepository
{
    public interface ISystemRepository
    {
        void UpdateMembershipCollection(ObservableCollection<MembershipModel> membershipsCollection);
    }
}