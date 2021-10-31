using chanosBot.Enum;
using chanosBot.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chanosBot.Model
{    
    public class AutoCommand : IEquatable<AutoCommand>
    {
        #region Properties
        public long ChatID { get; set; }
         
        public int UserID { get; set; }
         
        public EnumWeekValue Week { get; set; }
         
        public Time Time { get; set; } 
         
        public ICommand Command { get; set; }

        public string[] Options { get; set; }

        public bool IsSent { get; set; }
        #endregion

        #region override methods
        public override bool Equals(object obj)
        {
            return this.Equals(obj as AutoCommand);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"ChatID : {ChatID}, UserID : {UserID}, Command : {Command.CommandName}, Time : ({Time}), Option : {string.Join(", ", Options)}";
        }
        #endregion

        #region operator

        public static bool operator== (AutoCommand left, AutoCommand right)
        {
            if (left is null)
            {
                if (right is null)
                    return true;

                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(AutoCommand left, AutoCommand right)
        {
            return !(left == right);
        }

        #endregion

        #region IEquatable Interface
        public bool Equals(AutoCommand other)
        {
            if (other is null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;
            
            // Command interface는 한 개만 생성되기 때문에 Ref 비교로.
            return (this.ChatID == other.ChatID) && (this.UserID == other.UserID) && (this.Command == other.Command);
        }
        #endregion
    }

    public class AutoCommandComparer : IEqualityComparer<AutoCommand>
    {
        public bool Equals(AutoCommand x, AutoCommand y) => x.Equals(y);

        public int GetHashCode(AutoCommand obj) => base.GetHashCode();
    }
}
