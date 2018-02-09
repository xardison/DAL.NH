using System.Data;
using System.Linq;

namespace DAL.NH.Internal
{
    internal class DbHelper
    {
        public static void UpdateConnectionInfo(IDbConnection conn)
        {
            if (DalParams.ClientId == null && DalParams.ClientInfo == null &&
                DalParams.ModuleName == null && DalParams.ActionName == null)
            {
                return;
            }

            var props = conn.GetType().GetProperties();

            if (DalParams.ClientId != null)
            {
                var clientIdProp = props.FirstOrDefault(x => x.Name == "ClientId");
                if (clientIdProp != null)
                {
                    clientIdProp.SetValue(conn, DalParams.ClientId);
                }
            }

            if (DalParams.ClientInfo != null)
            {
                var clientInfoProp = props.FirstOrDefault(x => x.Name == "ClientInfo");
                if (clientInfoProp != null)
                {
                    clientInfoProp.SetValue(conn, DalParams.ClientInfo);
                }
            }

            if (DalParams.ModuleName != null)
            {
                var moduleNameProp = props.FirstOrDefault(x => x.Name == "ModuleName");
                if (moduleNameProp != null)
                {
                    moduleNameProp.SetValue(conn, DalParams.ModuleName);
                }
            }

            if (DalParams.ActionName != null)
            {
                var actionNameProp = props.FirstOrDefault(x => x.Name == "ActionName");
                if (actionNameProp != null)
                {
                    actionNameProp.SetValue(conn, DalParams.ActionName);
                }
            }
        }
    }
}