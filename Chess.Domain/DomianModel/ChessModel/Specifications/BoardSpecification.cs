using Microservice.Framework.Domain;
using Microservice.Framework.Domain.Rules.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.Specifications
{
    public class BoardSpecification : Specification<Board>
    {
        #region Virtual Methods

        protected override Notification IsNotSatisfiedBecause(Board obj)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
