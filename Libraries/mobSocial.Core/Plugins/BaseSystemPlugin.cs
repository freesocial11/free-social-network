namespace mobSocial.Core.Plugins
{
    public abstract class BaseSystemPlugin : BasePlugin
    {
        public override bool IsSystemPlugin
        {
            get { return true; }
        }

        public sealed override void Uninstall()
        {
            //can't uninstall system plugin
        }
    }
}