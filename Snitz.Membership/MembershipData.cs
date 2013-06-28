namespace SnitzMembership
{
    partial class Member
    {
        public bool IsLockedOut {
            get { return this.Status == 0; }
            set {
                this.Status = value ? 0 : 1;
            }
        }
    }
}
