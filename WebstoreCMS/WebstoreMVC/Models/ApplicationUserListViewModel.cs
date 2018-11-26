using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebstoreMVC.Models
{
    public class ApplicationUserListViewModel : ListViewModel<ApplicationUserListItemViewModel>
    {
        public ApplicationUserListViewModel() : base()
        {

        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RoleNames { get; set; }
        public string Admin_Role { get; set; }
        public string Storeowner_Role { get; set; }



}
}
