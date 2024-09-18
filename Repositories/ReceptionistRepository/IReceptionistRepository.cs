using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;

using Gym_Reception_Management_System.Models;
using Gym_Reception_Management_System.ViewModels;

namespace Gym_Reception_Management_System.Repositories.ReceptionistRepository
{
    public interface IReceptionistRepository
    {
        bool AuthenticateReceptAcc(NetworkCredential credential);
        bool CreateReceptAcc(ReceptionistAccountModel receptionistAccountModel);
        bool CreateMembership(MembershipModel membership, List<ServiceViewModel> servicesCollection);
        bool UpdateMembershipServices(List<ServiceViewModel> servicesCollection, int membershipId);
        bool AddMembershipServices(List<ServiceViewModel> newServices, int membershipId, int oldNoServices);
        bool DeleteMembership(MembershipModel selectedMembership);
    }
}