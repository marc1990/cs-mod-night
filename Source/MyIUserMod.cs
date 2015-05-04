using ICities;
using UnityEngine;
using ChirpLogger;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.Threading;
using System.Text;

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
			ChirpLog.Debug("Start draw_lights"); 
			Color kleur = Color.white;
			Vector3 position  = Vector3.zero;
			Quaternion orientation = Quaternion.identity;
			int i,j,d,m,ko;
			var cam_info = Camera.main;
			Light lightComp;
			int n = 0;
			float working = 0.0f;
			Vector3 speed;
			for(i = 0;i<vehicles.Length;i++){
				
				if ((vehicles[i].m_flags & Vehicle.Flags.Spawned) != 0 && n < max_cars && (ushort)i == vehicles[i].GetFirstVehicle((ushort)i)){
						
					vehicles[i].GetSmoothPosition((ushort)i, out position, out orientation);
					if(inrange(position)){
						Vector3 forward = orientation * Vector3.forward;
						Vector3 up = orientation * Vector3.up;
						
						float len = vehicles[i].CalculateTotalLength((ushort)i);
						if(len > 10){
							len = 10;
						}
						lichten[n].transform.position = position + 1f*up + len*forward*0.5f; 
						lichten[n].transform.LookAt(position + len*forward*1.0f); 
						
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
			
			ChirpLog.Debug(n + " Lights after cars"); 
			//ChirpLog.Flush();
			//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, n + " Lights after cars"); 
			Vector3 poss = Vector3.zero;
			Vector3 pose = Vector3.zero;
			Vector3 positions = Vector3.zero;
			Vector3 positione = Vector3.zero;
			Vector3 middlePos1 = Vector3.zero;
			Vector3 middlePos2 = Vector3.zero;
			float afstand,real_pitch;
			float lamps;
			/*
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
			*/
			
			ChirpLog.Debug(n + " Lights after props, lane props: " + count_lane_prop); 
			//ChirpLog.Flush();
			//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, n + " Lights after props, lane props: " + count_lane_prop); 
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
					//poss =  segments[m].m_startDirection;
					
					position = netMan.m_nodes.m_buffer[segments[m].m_endNode].m_position;
					segments[m].GetClosestPositionAndDirection(position,out positione,out pose);
					//pose =  segments[m].m_endDirection;
					NetSegment.CalculateMiddlePoints(positions,poss,positione,pose,true,true,out middlePos1,out middlePos2,out afstand);
					if(inrange(position)){
								if(segments[m].Info.m_lanes[i].m_laneProps.m_props[j].m_minLength < afstand){ 
									// 
									afstand = afstand - segments[m].Info.m_lanes[i].m_laneProps.m_props[j].m_segmentOffset;
									lamps = (int)((afstand)/(segments[m].Info.m_lanes[i].m_laneProps.m_props[j].m_repeatDistance ) + 0.53f );
									//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, " Lamps: " + segments[m].Info.m_lanes[i].m_stopOffset); 
									
									real_pitch = (afstand+0.5f*segments[m].Info.m_lanes[i].m_laneProps.m_props[j].m_repeatDistance)/lamps;
									
									
									if((segments[m].m_flags & NetSegment.Flags.End  ) ==NetSegment.Flags.End  ){
										working = real_pitch*0.5f;
										//afstand = afstand-real_pitch*0.5f;
										real_pitch = (afstand+0.5f*segments[m].Info.m_lanes[i].m_laneProps.m_props[j].m_repeatDistance-10f)/lamps;
										//lamps = 3;
									}else{
										working = 0; 
										real_pitch = (afstand+0.5f*segments[m].Info.m_lanes[i].m_laneProps.m_props[j].m_repeatDistance-20f)/lamps;  
										//lamps = 1;
									}
									
									for(ko = 0;ko < lamps;ko++){
										//if(working >  segments[m].Info.m_lanes[i].m_laneProps.m_props[j].m_segmentOffset){
											//- segments[m].Info.m_lanes[i].m_lawneProps.m_props[j].m_position 
											if(lamps == 1){ 
												 //ChirpLog.Debug("Flags: " + segments[m].m_flags);  
									//ChirpLog.Debug("m_startDirection: " + segments[m].m_startDirection); 
									//ChirpLog.Debug("m_endDirection: " + segments[m].m_endDirection); 
									//ChirpLog.Debug("poss: " + poss); 
									//ChirpLog.Debug("pose: " + pose); 
									//ChirpLog.Debug("m_verticalOffset: " + segments[m].Info.m_lanes[i].m_verticalOffset); 
												if((segments[m].m_flags & NetSegment.Flags.End  ) ==NetSegment.Flags.End  ){
													position = positions + ((afstand+10f)/2f)*segments[m].m_startDirection +segments[m].Info.m_lanes[i].m_laneProps.m_props[j].m_position; 
													 
												}else{
													position = positions + ((afstand)/2f)*poss +ko*Vector3.forward+segments[m].Info.m_lanes[i].m_laneProps.m_props[j].m_position;
												}
												lichten[n].transform.position = position + new Vector3(0f,10f,0f); 
												lichten[n].transform.LookAt(position);
												lichten[n].SetActive(true); 
												lightComp = lichten[n].GetComponent<Light>();
												lightComp.type = LightType.Spot;
												lightComp.intensity = 150; 
												lightComp.spotAngle = 150f; 
												lightComp.color = kleur;
												lightComp.range = 15f; 
												n++; 
											}
											//working = working - segments[m].Info.m_lanes[i].m_laneProps.m_props[j].m_repeatDistance;
										
										
										if(lamps > 1 ){
											//center at = (afstand)/2f
											if((segments[m].m_flags & NetSegment.Flags.End  ) ==NetSegment.Flags.End  ){
												position = positions + (afstand/2f - real_pitch/2f + real_pitch*ko + 5f)*poss  ; 
											}else{
												position = positions + (afstand/2f - real_pitch/2f + real_pitch*ko)*poss  ; 
											}
											lichten[n].transform.position = position + new Vector3(0f,10f,0f); 
											lichten[n].transform.LookAt(position);
											lichten[n].SetActive(true); 
											lightComp = lichten[n].GetComponent<Light>();
											lightComp.type = LightType.Spot;
											lightComp.intensity = 6; 
											lightComp.spotAngle = 150f; 
											lightComp.color = kleur;
											lightComp.range = 15f;
											n++; 
											//working = working + real_pitch;
										}
									}
								}
							
						
					}
					}
				//}
				
				if(n > max_cars){
					d = 100000;
				 }
			}
			ChirpLog.Debug(n + " Lights !"); 
			//ChirpLog.Flush();
			for(i = n;n<256;n++){
				lichten[n].SetActive(false);
				
			}
			ChirpLog.Debug("Done"); 
			//ChirpLog.Flush();
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
			//ChirpLog.Debug("There were "+count_build_prop + " building props lamps.");
           // ChirpLog.Flush();
		}
		public void day(){
			int n,i;
			//lane_props = new int[max_lane_props*3];
			//count_lane_prop = 0;
			n = 0;
			//updated_segment_last = 0;
			//count_build_prop = 0;
			for(i = n;n<max_cars;n++){
				lichten[n].SetActive(false);
				
			}
		}
		public void start_game(){
			//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, " Begin init"); 
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
			max_cars = 32;//is equal to max number of lights
			//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, max_cars + " lights add!"); 
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
			//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "Lanes props added!"); 
			road_props_list();
			//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "Road prop start build, done!"); 
			//build_props_list(); 
			//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "prop build, done!"); 
			//show_textuers();
			//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "All " + Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length);
		
		//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "AudioClips " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length);
		//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "Meshes " + Resources.FindObjectsOfTypeAll(typeof(Mesh)).Length);
		//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "Materials " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length);
		//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "GameObjects " + Resources.FindObjectsOfTypeAll(typeof(GameObject)).Length);
		//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "Components " + Resources.FindObjectsOfTypeAll(typeof(Component)).Length);
			
			
		}
		public void show_textuers(){
			List<Material> allObjects = new List<Material>( GameObject.FindObjectsOfType<Material>() );
			string result = "";
			int c = 0;
			foreach(Material obj in allObjects)
		  {
			if(c > 0 && c < 2000){ 
			result = "Name: "+ obj.name;
			DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, result);
			//result = "_MainTex: "+ obj.GetTexture("_MainTex");  
			//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, result);
			//result = "_BumpMap: "+ obj.GetTexture("_BumpMap");  
			//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, result);
			result = "_Cube: "+ obj.GetTexture("_Cube");  
			DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, result);
			}
			c++;
			//result += "height: "+ obj.height + "\n"; 
			//result += "\n";
		  }
		  
		}
		static string ConvertStringArrayToString(string[] array)
    {
	//
	// Concatenate all the elements into a StringBuilder.
	//
	StringBuilder builder = new StringBuilder();
	foreach (string value in array)
	{
	    builder.Append(value);
	    builder.Append('.');
	}
	return builder.ToString();
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
										//ChirpLog.Debug("i:"+i);
										//ChirpLog.Debug("m_angle:"+segments[n].Info.m_lanes[i].m_laneProps.m_props[j].m_angle);
										//ChirpLog.Debug("m_segmentOffset:"+segments[n].Info.m_lanes[i].m_laneProps.m_props[j].m_segmentOffset);
										//ChirpLog.Debug("m_repeatDistance:"+segments[n].Info.m_lanes[i].m_laneProps.m_props[j].m_repeatDistance);
										//ChirpLog.Debug("m_minLength:"+segments[n].Info.m_lanes[i].m_laneProps.m_props[j].m_minLength);
										//ChirpLog.Debug("m_cornerAngle:"+segments[n].Info.m_lanes[i].m_laneProps.m_props[j].m_cornerAngle);
										//ChirpLog.Debug("m_probability:"+segments[n].Info.m_lanes[i].m_laneProps.m_props[j].m_probability);
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
			//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "Road props: "+max_lane_props+"!"); 
			//ChirpLog.Flush();
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
		public float get_info_main_light(){
			List<Light> allObjects = new List<Light>( GameObject.FindObjectsOfType<Light>() );
			foreach(Light obj in allObjects)
			{
				
				if(obj.name == "Directional Light"){
					return obj.transform.localEulerAngles.x; 
				}
			}
			return 0.0f;
		}
    }
	public class MyIUserMod1: IThreadingExtension
    {
		private MyIUserMod _mod;
		private bool _mod_loaded = false;
		private bool night = false;
		private float avg_fps = 0f;
		private int frame_counter = 0;
		public void OnCreated(IThreading threading)
		{
			//ChirpLog.Debug("IThreading Created");
		 
			
		}
		public void OnReleased()
		{
		
		}
		
		public void OnUpdate(float realTimeDelta, float simulationTimeDelta)
		{
			try
{
	ChirpLog.Debug("Start with all"); 
				//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, " fps: " + 1f/realTimeDelta); 
			//check init
			if(!_mod_loaded){
				_mod = new MyIUserMod();
				if(_mod.Dim_all() > 0){ 
					
					//ChirpLog.Debug("StartGame");
					//ChirpLog.Flush();
					_mod.start_game();
					_mod_loaded = true;
					//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, " Light level: " + _mod.get_info_main_light()); 
					
				}
			}else{
					ChirpLog.Debug("Delta"); 
				avg_fps = avg_fps*0.97f+1f/realTimeDelta*0.03f;
				if(  _mod.max_cars < 200){
					if(avg_fps > 35){
						_mod.max_cars = _mod.max_cars + 1;
						//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "Light max:" + _mod.max_cars); 
					}
				}
				if( _mod.max_cars > 32){
					if(avg_fps < 30){
						_mod.max_cars = _mod.max_cars - 1;
						//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, "Light max:" + _mod.max_cars + " reduced"); 
					}
				}
				ChirpLog.Debug("A"); 
				if(_mod.updated_segment_last <_mod.segments.Length && _mod.count_build_prop < _mod.max_lane_props){ 
					_mod.road_props_list(_mod.updated_segment_last,_mod.updated_segment_last+500);
					_mod.updated_segment_last = _mod.updated_segment_last + 500;
					//_mod.updated_segment_last = 0;
					//_mod.count_build_prop = 0;
				}
				ChirpLog.Debug("B"); 
				//DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Message, " Light level: " + _mod.get_info_main_light()); 
				if(frame_counter > 10){
					if(_mod.get_info_main_light() < 40){
						night = true;
					}else{
						night = false;
					}
					frame_counter = 0;
				}
				frame_counter++;
				if(night){
					//slow road updater
				ChirpLog.Debug("C"); 
				
					_mod.Update();
				}else{
					_mod.day();
				}
				
				ChirpLog.Debug("Done with all"); 
				//ChirpLog.Flush();
			}
			}catch
{
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
