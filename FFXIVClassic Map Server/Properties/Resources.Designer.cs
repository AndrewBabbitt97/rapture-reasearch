﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FFXIVClassic_Map_Server.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FFXIVClassic_Map_Server.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Adds the specified currency to the current player&apos;s inventory
        ///
        ///*Syntax:	givecurrency &lt;quantity&gt;
        ///		givecurrency &lt;quantity&gt; &lt;type&gt;
        ///&lt;type&gt; is the specific type of currency desired, defaults to gil if no type specified.
        /// </summary>
        public static string CPgivecurrency {
            get {
                return ResourceManager.GetString("CPgivecurrency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Adds the specified items to the current player&apos;s inventory
        ///
        ///*Syntax:	giveitem &lt;item id&gt;
        ///		giveitem &lt;item id&gt; &lt;quantity&gt;
        ///		giveitem &lt;item id&gt; &lt;quantity&gt; &lt;type&gt;
        ///&lt;item id&gt; is the item&apos;s specific id as defined in the server database
        ///&lt;type&gt; is the type as defined in the server database (defaults to standard item if not specified).
        /// </summary>
        public static string CPgiveitem {
            get {
                return ResourceManager.GetString("CPgiveitem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Adds the specified key item to the current player&apos;s inventory
        ///
        ///*Syntax:	givekeyitem &lt;item id&gt;
        ///&lt;item id&gt; is the key item&apos;s specific id as defined in the server database.
        /// </summary>
        public static string CPgivekeyitem {
            get {
                return ResourceManager.GetString("CPgivekeyitem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use !help(command) for details
        ///
        ///Available commands: 
        ///Standard: mypos, music, warp
        ///Server Administration: givecurrency, giveitem, givekeyitem, removecurrency, removekeyitem, reloaditems, reloadzones.
        /// </summary>
        public static string CPhelp {
            get {
                return ResourceManager.GetString("CPhelp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Changes the currently playing background music
        ///
        ///*Syntax:	music &lt;music id&gt;
        ///&lt;music id&gt; is the key item&apos;s specific id as defined in the server database.
        /// </summary>
        public static string CPmusic {
            get {
                return ResourceManager.GetString("CPmusic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Prints out your current location
        ///
        ///*Note: The X/Y/Z coordinates do not correspond to the coordinates listed in the in-game map, they are based on the underlying game data.
        /// </summary>
        public static string CPmypos {
            get {
                return ResourceManager.GetString("CPmypos", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Syntax:	property &lt;value 1&gt; &lt;value 2&gt; &lt;value 3&gt;.
        /// </summary>
        public static string CPproperty {
            get {
                return ResourceManager.GetString("CPproperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Syntax:	property2 &lt;value 1&gt; &lt;value 2&gt; &lt;value 3&gt;.
        /// </summary>
        public static string CPproperty2 {
            get {
                return ResourceManager.GetString("CPproperty2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reloads the current item data from the database.
        /// </summary>
        public static string CPreloaditems {
            get {
                return ResourceManager.GetString("CPreloaditems", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reloads the current zone data from the database.
        /// </summary>
        public static string CPreloadzones {
            get {
                return ResourceManager.GetString("CPreloadzones", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Removes the specified currency from the current player&apos;s inventory
        ///
        ///*Syntax:	removecurrency &lt;quantity&gt;
        ///		removecurrency &lt;quantity&gt; &lt;type&gt;
        ///&lt;type&gt; is the specific type of currency desired, defaults to gil if no type specified.
        /// </summary>
        public static string CPremovecurrency {
            get {
                return ResourceManager.GetString("CPremovecurrency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Removes the specified items to the current player&apos;s inventory
        ///
        ///*Syntax:	removeitem &lt;itemid&gt;
        ///		removeitem &lt;itemid&gt; &lt;quantity&gt;
        ///&lt;item id&gt; is the item&apos;s specific id as defined in the server database.
        /// </summary>
        public static string CPremoveitem {
            get {
                return ResourceManager.GetString("CPremoveitem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Removes the specified key item to the current player&apos;s inventory
        ///
        ///*Syntax:	removekeyitem &lt;itemid&gt;
        ///&lt;item id&gt; is the key item&apos;s specific id as defined in the server database.
        /// </summary>
        public static string CPremovekeyitem {
            get {
                return ResourceManager.GetString("CPremovekeyitem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server sends a special packet to the client
        ///
        ///*Syntax:	sendpacket &lt;path to packet&gt;
        ///&lt;Path to packet&gt; is the path to the packet, starting in &lt;map server install location&gt;\packet.
        /// </summary>
        public static string CPsendpacket {
            get {
                return ResourceManager.GetString("CPsendpacket", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Overrides the currently displayed character equipment in a specific slot
        ///
        ///*Note: Similar to Glamours in FFXIV:ARR, the overridden graphics are purely cosmetic, they do not affect the underlying stats of whatever is equipped on that slot
        ///
        ///*Syntax:	sendpacket &lt;slot&gt; &lt;wid&gt; &lt;eid&gt; &lt;vid&gt; &lt;cid&gt;
        ///&lt;w/e/v/c id&gt; are as defined in the client game data.
        /// </summary>
        public static string CPsetgraphic {
            get {
                return ResourceManager.GetString("CPsetgraphic", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Teleports the player to the specified location
        ///
        ///*Note: You can teleport relative to your current position by putting a @ in front of a value, cannot be combined with a zone id or instance name
        ///
        ///*Syntax:	warp &lt;location list&gt;
        ///		warp &lt;X coordinate&gt; &lt;Y coordinate&gt; &lt;Z coordinate&gt;
        ///		warp &lt;zone id&gt; &lt;X coordinate&gt; &lt;Y coordinate&gt; &lt;Z coordinate&gt;
        ///		warp &lt;zone id&gt; &lt;instance&gt; &lt;X coordinate&gt; &lt;Y coordinate&gt; &lt;Z coordinate&gt;
        ///&lt;location list&gt; is a pre-defined list of locations from the server database
        ///&lt;zone id&gt; is the [rest of string was truncated]&quot;;.
        /// </summary>
        public static string CPwarp {
            get {
                return ResourceManager.GetString("CPwarp", resourceCulture);
            }
        }
    }
}
