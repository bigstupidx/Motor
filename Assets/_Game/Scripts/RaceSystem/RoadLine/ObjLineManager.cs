using UnityEngine.SceneManagement;
using UnityEngine;
using XPlugin.Update;

namespace Game {
	public class ObjLineManager : LineManagerBase<ObjLine, ObjLineManager> {

		public override ObjLine SpawnLine(string line) {
			var scene = SceneManager.GetActiveScene().name;
			scene = scene.Split('_')[0];
            var prefab = UResources.Load<GameObject>(scene + "/objline/" + line);
            if (prefab == null)
            {
                return null;
            }
            else
            {
                return base.SpawnLine(scene + "/objline/" + line);
            }
		}
	}
}