using ICities;
using UnityEngine;
using ChirpLogger;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.Threading;

namespace DemoMod
{

    public class MyIUserMod: IUserMod
    {
		VehicleManager vehicleManager = Singleton<VehicleManager>.instance;
		BuildingManager buildingManager = Singleton<BuildingManager>.instance;
		NetManager netMan = Singleton<NetManager>.instance;
		private Vehicle[] vehicles;
		private Building[] buildings;
		public NetSegment[] segments;
		private GameObject[] lichten;
		private int[] build_props,lane_props;
		public int max_cars,max_lane_props,max_building_props;
		public int count_build_prop,count_lane_prop;
		public int updated_segment_last;
		
        public string Name 
        {
            get { return "Light up the city"; } 
        }
 
        public string Description 
        {
            get { return "This mod will add lights to city and darken the sun."; }
			
        }
		public void Init()
        {
		}
		
		public void add_light(Vector3 pos,Quaternion rot,int i,int t){
			// Make a game object
			lichten[t] = new GameObject("car_light_" + i);
			var lightGameObject = lichten[t];
			// = lightGameObject.GetComponent<Light>();
			
			// Add the light component
			Light lightComp = lightGameObject.AddComponent<Light>();
			lightComp.shadows = LightShadows.None;
			lightGameObject.SetActive(false); 
		}
		public void draw_lights(int mode){
			ChirpLog.Debug("Start"); 
			Color kleur = Color.white;
			Vector3 position  = Vector3.zero;
			Quaternion orientation = Quaternion.identity;
			int i,j,d,m;
			var cam_info = Camera.main;
			Light lightComp;
			int n = 0;
			Vector3 speed;
			for(i = 0;i<vehicles.Length;i++){
				
				if ((vehicles[i].m_flags & Vehicle.Flags.Spawned) != 0 && n < max_cars && (ushort)i == vehicles[i].GetFirstVehicle((ushort)i)){
						
					vehicles[i].GetSmoothPosition((ushort)i, out position, out orientation);
					if(inrange(position)){
						Vector3 forward = orientation * Vector3.forward;
						Vector3 up = orientation * Vector3.up;
						Vector3 right = orientation * Vector3.right;
						float len = vehicles[i].CalculateTotalLength((ushort)i);
						if(len > 10){
							len = 10;
						}
						lichten[n].transform.position = position + 1f*up + len*forward*0.5f; 
						lichten[n].transform.LookAt(position + len*forward*1.0f); 
						//lichten[n].transform.Rotate(right*90);
						lichten[n].SetActive(true);  
						lightComp = lichten[n].GetComponent<Light>();
						lightComp.type = LightType.Spot;
						lightComp.spotAngle = 50f;
						lightComp.intensity = 8; 
						kleur.r = 1f; 
						kleur.g = 0.96f;
						kleur.b = 0.9f;
						kleur.a = 1f;
						lightComp.color = kleur;
						n++;
						lightComp.range = 10f;

						}
						
					
					
				 }
				 
				 if ((vehicles[i].m_flags & Vehicle.Flags.Spawned) != 0 && n < max_cars && (ushort)i == vehicles[i].GetLastVehicle((ushort)i) && cam_info.transform.position.y < 300){
					speed = vehicles[i].GetLastFrameVelocity() ;
					vehicles[i].GetSmoothPosition((ushort)i, out position, out orientation);
					if(inrange(position)  && speed.magnitude<6){
						Vector3 forward = orientation * Vector3.forward;
						Vector3 up = orientation * Vector3.up;
						Vector3 right = orientation * Vector3.right;
						float len = vehicles[i].CalculateTotalLength((ushort)i);
						if(len > 10){
							len = 10;
						}
						lichten[n].transform.position = position + 1f*up - len*forward*0.5f; 
						lichten[n].transform.LookAt(position - len*forward*1.0f);
						//lichten[n].transform.Rotate(right*20);
						//lichten[n].transform.Rotate(forward*180);
						lichten[n].SetActive(true);  
						lightComp = lichten[n].GetComponent<Light>();
						lightComp.type = LightType.Spot;
						lightComp.spotAngle = 60f;
						
						//ChirpLog.Debug((speed.x+speed.y+speed.z) +" break?"); 
			
						lightComp.intensity =6-speed.magnitude; 
						kleur =  Color.red;
						lightComp.color = kleur;
						lightComp.range = 10f; 
						n++;

						}
						
					
					
				 }
				 
				 
				 
				 
				 if(n == max_cars){
					i = 100000;
				 }
			}
			//ChirpLog.Debug(n + " Lights after cars"); 
			//ChirpLog.Flush();
			
			Vector3 poss = Vector3.zero;
			Vector3 pose = Vector3.zero;
			Vector3 positions = Vector3.zero;
			Vector3 positione = Vector3.zero;
			Vector3 middlePos1 = Vector3.zero;
			Vector3 middlePos2 = Vector3.zero;
			float afstand;
			//ChirpLog.Flush();
			kleur.r = 1f;  
			kleur.g = 0.96f;
			kleur.b = 0.9f;
			kleur.a = 1f;
			
			//ChirpLog.Debug(n + " cars"); 
			for(d = 0;d < count_build_prop;d++){
				i = build_props[d];
				j = build_props[d+max_building_props];
				if(buildings[i].Info != null){
					var propsjes = buildings[i].Info.m_props;
					if(propsjes[j].m_prop != null){
						position = propsjes[j].m_position;
						position.z = -position.z ;
						position = buildings[i].CalculatePosition(position);
						if(inrange(position)){
							lichten[n].transform.position = position +  new Vector3(0f,3f,0f); 
							lichten[n].transform.LookAt(position);
							lichten[n].SetActive(true);
							lightComp = lichten[n].GetComponent<Light>();
							lightComp.type = LightType.Spot;
							lightComp.intensity = 5; 
							lightComp.spotAngle = 170f;
							lightComp.color = kleur;
							lightComp.range = 10f;
							n++;
						}
					}
				}
				if(n == max_cars){
					d = 100000;
				 }
			}
			//ChirpLog.Debug(n + " Lights after props, lane props: " + count_lane_prop); 
			//ChirpLog.Flush();
			//road lights
			kleur.r = 1f; 
			kleur.g = 0.96f;
			kleur.b = 0.7f;
			kleur.a = 1f;
			for(d = 0;d < count_lane_prop;d++){
				//ChirpLog.Debug(d + " regel"); 
				m = lane_props[d];
				i = lane_props[d+max_lane_props];
				j = lane_props[d+max_lane_props*2];
				if(segments[m].Info.m_lanes[i].m_laneProps.m_props[j].m_prop != null){
					position = netMan.m_nodes.m_buffer[segments[m].m_startNode].m_position;
					segments[m].GetClosestPositionAndDirection(position,out positions,out poss);
					position = netMan.m_nodes.m_buffer[segments[m].m_endNode].m_position;
					segments[m].GetClosestPositionAndDirection(position,out positione,out pose);
					NetSegment.CalculateMiddlePoints(positions,poss,positione,pose,true,true,out middlePos1,out middlePos2,out afstand);
					if(inrange(position)){
						
							if(afstand < 80f){
								position = positions+poss*afstand/2 ;
								lichten[n].transform.position = position + new Vector3(0f,5f,0f); 
								lichten[n].transform.LookAt(position);
								lichten[n].SetActive(true);
								lightComp = lichten[n].GetComponent<Light>();
								lightComp.type = LightType.Spot;
								lightComp.intensity = 6; 
								lightComp.spotAngle = 150f;
								lightComp.color = kleur;
								lightComp.range = 15f;
								n++;
							}else{
								position = positions+poss*20f;
								lichten[n].transform.position = position + new Vector3(0f,5f,0f); 
								lichten[n].transform.LookAt(position);
								lichten[n].SetActive(true);
								lightComp = lichten[n].GetComponent<Light>();
								lightComp.type = LightType.Spot;
								lightComp.intensity = 6; 
								lightComp.spotAngle = 150f;
								lightComp.color = kleur;
								lightComp.range = 15f;
								n++;
								position = positione-pose*20f;
								lichten[n].transform.position = position + new Vector3(0f,5f,0f); 
								lichten[n].transform.LookAt(position);
								lichten[n].SetActive(true);
								lightComp = lichten[n].GetComponent<Light>();
								lightComp.type = LightType.Spot;
								lightComp.intensity = 6; 
								lightComp.spotAngle = 150f;
								lightComp.color = kleur;
								lightComp.range = 15f;
								n++;
							}
						
					}
					}
				//}
				
				if(n == max_cars || n+1 == max_cars || n+2 == max_cars || n+3 == max_cars){
					d = 100000;
				 }
			}
			
			for(i = n;n<max_cars;n++){
				lichten[n].SetActive(false);
				
			}
		}
		//will check if the object is on screen + marge(s)
		public bool inrange(Vector3 position){
			var origin = Camera.main.WorldToScreenPoint (position); 
			var cam_info = Camera.main;
			float s = 1.1f; 
			if(origin.x < 1920*s && origin.x > -1920*(s-1) && origin.y < 1080*s && origin.y > -1080*(s-1) && origin.z < 500+cam_info.transform.position.y*1.4f && origin.z > -10){
				return true;
			}
			return false;
		}
		//Build prop list. Is run at start. The function takes some time, so it shouldn't run everytime a frame is render.
		public void build_props_list(){
			int i,j;
			int n=0; 
			for(i = 0;i<buildings.Length;i++){
				if(buildings[i].Info != null){
					if(buildings[i].Info.m_alreadyUnlocked == false){
						var propsjes = buildings[i].Info.m_props;
						for(j = 0;j<propsjes.Length;j++){
							if(propsjes[j].m_prop != null){
								if(propsjes[j].m_prop.GetLocalizedTitle() == "Street Lamp #2" || propsjes[j].m_prop.GetLocalizedTitle() == "Street Lamp #1"){
									build_props[n] = i; 
									build_props[n+max_building_props] = j;
									n++;
								}
							}
						}
					}
				}
				if(n == max_building_props){
					i = 100000;
				}
			}
			count_build_prop = n;
			ChirpLog.Debug("There were "+count_build_prop + " building props lamps.");
            ChirpLog.Flush();
		}
		public void start_game(){
			DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, " Begin init"); 
			vehicles = vehicleManager.m_vehicles.m_buffer;
			buildings = buildingManager.m_buildings.m_buffer;
			segments = netMan.m_segments.m_buffer;
			clean_lights();
			Dim_all();
			int n=0;
			Vector3 position  = Vector3.zero;
			Quaternion orientation = Quaternion.identity;
			max_cars = 256;//is equal to max number of lights
			max_lane_props = 2048 ; 
			max_building_props = 256;
			lichten = new GameObject[max_cars];
			build_props = new int[max_building_props*2];
			lane_props = new int[max_lane_props*3];
			for(n = 0;n<max_cars;n++){
				add_light(position,orientation,n,n);
			}
			DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, max_cars + " lights add!"); 
			//init build props array
			for(n = 0;n<max_building_props;n++){
				build_props[n] = 0; 
				build_props[n+max_building_props] = 0;
			}
			//init lane props array
			for(n = 0;n<max_lane_props;n++){
				lane_props[n] = 0;
				lane_props[n+max_lane_props] = 0;
				lane_props[n+max_lane_props*2] = 0;
			}
			DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "Lanes props added!"); 
			road_props_list();
			DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "Road prop start build, done!"); 
			build_props_list();
			DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "prop build, done!"); 
		}
		public void road_props_list(){
			count_lane_prop = 0;
			road_props_list(0,100);
		}
		public void road_props_list(int start,int end){
			int n,i,j,k;
			k = count_lane_prop;
			for(n = start;n<end&&n<segments.Length&&k<max_lane_props;n++){ 
				if(segments[n].Info != null && (
				   segments[n].Info.GetLocalizedTitle() == "Two-Lane Road" 
				|| segments[n].Info.GetLocalizedTitle() == "Two-Lane Road with Decorative Trees"
				|| segments[n].Info.GetLocalizedTitle() == "Two-Lane Road Road with Grass"
				|| segments[n].Info.GetLocalizedTitle() == "Two-Lane One-Way Road with Decorative Trees"
				|| segments[n].Info.GetLocalizedTitle() == "Two-Lane One-Way Road with Grass"
				|| segments[n].Info.GetLocalizedTitle() == "Two-Lane One-Way Road"
				
				
				)){
					/*
					
					|| segments[n].Info.GetLocalizedTitle() == "Four-Lane One-Way Road with Decorative Trees"
					|| segments[n].Info.GetLocalizedTitle() == "Four-Lane One-Way Road with Grass"
					|| segments[n].Info.GetLocalizedTitle() == "Four-Lane One-Way Road"
					|| segments[n].Info.GetLocalizedTitle() == "Four-Lane Road with Decorative Trees"
					|| segments[n].Info.GetLocalizedTitle() == "Four-Lane Road with Grass"
					|| segments[n].Info.GetLocalizedTitle() == "Four-Lane Road"
					
					|| segments[n].Info.GetLocalizedTitle() == "Six-Lane One-Way Road with Decorative Trees"
					|| segments[n].Info.GetLocalizedTitle() == "Six-Lane One-Way Road with Grass"
					|| segments[n].Info.GetLocalizedTitle() == "Six-Lane One-Way Road"
					|| segments[n].Info.GetLocalizedTitle() == "Six-Lane Road with Decorative Trees"
					|| segments[n].Info.GetLocalizedTitle() == "Six-Lane Road with Grass"
					|| segments[n].Info.GetLocalizedTitle() == "Six-Lane Road"
					*/
					for(i = 0;i<15&&i<segments[n].Info.m_lanes.Length;i++){
						if(segments[n].Info.m_lanes[i].m_laneProps != null){
							for(j = 0;j<40&&j<segments[n].Info.m_lanes[i].m_laneProps.m_props.Length;j++){ 
								if(segments[n].Info.m_lanes[i].m_laneProps.m_props[j].m_prop != null){
									if(segments[n].Info.m_lanes[i].m_laneProps.m_props[j].m_prop.GetLocalizedTitle() == "PROPS_TITLE[New Street Light]:0"){
										lane_props[k] = n;
										lane_props[k+max_lane_props] = i;
										lane_props[k+max_lane_props*2] = j; 
										k++;
										if(k == max_lane_props){
											count_lane_prop = k; 
											//ChirpLog.Debug("road_props:"+count_lane_prop);
											//ChirpLog.Flush();
											return;
										}
									}
								}
							}
						}
					}
				}
			}
			count_lane_prop = k; 
			DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "Road props: "+max_lane_props+"!"); 
		}
		//This function will give some info over the in game lights
		public string light_info()
		{
		  List<Light> allObjects = new List<Light>( GameObject.FindObjectsOfType<Light>() );
		  

		  string result = "";

		  foreach(Light obj in allObjects)
		  {
			result += "Name: "+ obj.name + "\n";
			result += "color: "+ obj.color + "\n";
			result += "intensity: "+ obj.intensity + "\n";
			result += "range: "+ obj.range + "\n";
			result += "spotAngle: "+ obj.spotAngle + "\n";
			result += "transform.position: "+ obj.transform.position + "\n";
			result += "transform.rotation: "+ obj.transform.rotation + "\n";
			result += "type: "+ obj.type + "\n";
			result += "renderMode: "+ obj.renderMode + "\n";
			result += "bounceIntensity: "+ obj.bounceIntensity + "\n";
			result += "flare: "+ obj.flare + "\n";
			result += "shadowBias: "+ obj.shadowBias + "\n";
			result += "shadows: "+ obj.shadows + "\n";
			result += "shadowStrength: "+ obj.shadowStrength + "\n";
			result += "enabled: "+ obj.enabled + "\n";
			result += "tag: "+ obj.tag + "\n";
			result += "rot: "+ obj.transform.localEulerAngles + "\n";
			result += "\n";
		  }
		  return result;
		}
		
		public void Update(){
			
			 draw_lights(0); 
			
		}
		
		//keep only the sun
		public void clean_lights(){
			List<Light> allObjects = new List<Light>( GameObject.FindObjectsOfType<Light>() );
			foreach(Light obj in allObjects)
			{
				if(obj.name != "Directional Light"){
					GameObject.Destroy(GameObject.Find(obj.name));
				}
			}
		}
		//debug function
		public string DumpAllGameObjects()
		{
		  List<GameObject> allObjects = new List<GameObject>( GameObject.FindObjectsOfType<GameObject>() );
		  string result = "";
		  foreach(GameObject obj in allObjects)
		  {
			result += obj.name + "\n";
		  }
		  return result;
		}

		//turn all lights to intensity of 0.7
		public int Dim_all()
		{	
			int count = 0;
			List<Light> allObjects = new List<Light>( GameObject.FindObjectsOfType<Light>() );
			foreach(Light obj in allObjects)
			{
				obj.intensity = 0.7f;
				count++;
			}
			return count;
		}
    }
	public class MyIUserMod1: IThreadingExtension
    {
		private MyIUserMod _mod;
		private bool _mod_loaded = false;
		
		public void OnCreated(IThreading threading)
		{
			ChirpLog.Debug("IThreading Created");
		 
			
		}
		public void OnReleased()
		{
		
		}
		
		public void OnUpdate(float realTimeDelta, float simulationTimeDelta)
		{
			//check init
			if(!_mod_loaded){
				_mod = new MyIUserMod();
				if(_mod.Dim_all() > 0){ 
					
					ChirpLog.Debug("StartGame");
					ChirpLog.Flush();
					_mod.start_game();
					_mod_loaded = true;
				}
			}else{
				_mod.Update();
				//slow road updater
				if(_mod.updated_segment_last <_mod.segments.Length && _mod.count_build_prop < _mod.max_lane_props){ 
					_mod.road_props_list(_mod.updated_segment_last,_mod.updated_segment_last+150);
					_mod.updated_segment_last = _mod.updated_segment_last + 150;
					//_mod.updated_segment_last = 0;
					//_mod.count_build_prop = 0;
				}
				
				
			}
			
		}
		//Thread: Simulation
		public void OnBeforeSimulationTick()
		{
			
		}
		
		//Thread: Simulation
		public void OnAfterSimulationFrame()
		{
			
		}
		//Thread: Simulation
		public void OnAfterSimulationTick()
		{
		
		}
		public void OnBeforeSimulationFrame()
		{
			
			
		}
	}
}
