using GameFramework.Procedure;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace MO.Unity3d.Procedure
{
    public class ProcedureSplash : ProcedureBase
    {
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            // TODO: 这里可以播放一个 Splash 动画
            // ...
            if (GameEntry.Base.EditorResourceMode)
            {
                // 编辑器模式
                Log.Info("Editor resource mode detected.");
                ChangeState<ProcedureLogin>(procedureOwner);
            }
            else if (GameEntry.Resource.ResourceMode == ResourceMode.Package)
            {
                // 单机模式
                Log.Info("Package resource mode detected.");
                ChangeState<ProcedureInitResources>(procedureOwner);
            }
            else
            {
                // 可更新模式
                Log.Info("Updatable resource mode detected.");
                //ChangeState<ProcedureCheckVersion>(procedureOwner);
            }
        }
    }
}
