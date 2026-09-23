namespace RogueShootingAdventure
{
    public class RogueShootingAdventureMgr : MonoBehaviour
    {
        public RogueShootingAdventureMgr(IntPtr ptr) : base(ptr) { }
        public RogueShootingAdventureMgr() : base(ClassInjector.DerivedConstructorPointer<RogueShootingAdventureMgr>())
        {
            ClassInjector.DerivedConstructorBody(this);
        }
        public static RogueShootingAdventureMgr __instance { get; private set; }
    }
}