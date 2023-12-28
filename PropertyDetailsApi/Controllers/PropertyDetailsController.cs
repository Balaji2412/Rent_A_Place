using PropertyDetailsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PropertyDetailsApi.Controllers
{
    public class PropertyDetailsController : ApiController
    {
        //show all the properties availabe in the database to the user

        //http://localhost:62630/api/Propertydetails
        public IEnumerable<propertyDetail> GetPropertyDetails()
        {
            IList<propertyDetail> pd = null;
            using (capstoneProjectEntities cp = new capstoneProjectEntities())
            {
                pd = cp.propertyDetails.ToList();
            }
            return pd;
        }

        //http://localhost:62630/api/Propertydetails?location=vellore
        public IEnumerable<propertyDetail> getpropertylocation(string location)
        {
            IList<propertyDetail> pd = null;
            using (capstoneProjectEntities cp = new capstoneProjectEntities())
            {
                if (location == null)
                {
                    pd = cp.propertyDetails.ToList();
                }
                else
                {
                    pd = cp.propertyDetails
                        .Where(p => p.Location.ToLower() == location.ToLower())
                        .ToList();
                }
            }
            return pd;
        }

        public IEnumerable<propertyDetail> getpropertType(string propertyname)
        {
            IList<propertyDetail> pd = null;
            using (capstoneProjectEntities cp = new capstoneProjectEntities())
            {
                if (propertyname == null)
                {
                    pd = cp.propertyDetails.ToList();
                }
                else
                {
                    pd = cp.propertyDetails
                        .Where(p => p.propertyName.ToLower() == propertyname.ToLower())
                        .ToList();
                }
            }
            return pd;
        }


        public IEnumerable<propertyDetail> getpropertfeatures(string features)
        {
            IList<propertyDetail> pd = null;
            using (capstoneProjectEntities cp = new capstoneProjectEntities())
            {
                if (features == null)
                {
                    pd = cp.propertyDetails.ToList();
                }
                else
                {
                    pd = cp.propertyDetails
                        .Where(p => p.category.ToLower() == features.ToLower())
                        .ToList();
                }
            }
            return pd;
        }
    }
}
