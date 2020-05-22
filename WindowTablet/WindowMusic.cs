using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using CellBig.Contents;

public class WindowMusic : MonoBehaviour
{
    private static WindowMusic instance;
    private static GameObject contain;
    //Playlist playlist = null;
    public static WindowMusic GetInstance()
    {
        if (!instance)
        {
            contain = new GameObject();
            contain.name = "WindowMusic";
            instance = contain.AddComponent(typeof(WindowMusic)) as WindowMusic;
        }

        return instance;
    }


    //public List<Music> listMusic;

    // Use this for initialization
    public Playlist GetPlaylist()
    {
        Playlist playlist = new Playlist();
        //playlist.data = new List<Music>();
        string[] path = Application.dataPath.Split('/');
        //listMusic = new List<Music>();

#if (UNITY_EDITOR)
        string tempPath = Application.dataPath + "/Music";
        DirectoryInfo di = new DirectoryInfo(tempPath);
        MusicPlayLisy(di, tempPath, out playlist);
#elif (UNITY_STANDALONE_WIN)
        string  tempPath = path[0] + "/" + path[1] + "/" + path[2] + "/" + path[3] + "/" + path[4] + "/Music";
        DirectoryInfo di = new DirectoryInfo(tempPath);
        MusicPlayLisy(di, tempPath, out playlist);
#endif
        return playlist;
    }


    void MusicPlayLisy(DirectoryInfo di, string path, out Playlist playlist)
    {
        if (di.Exists)
        {
            Playlist tempPlayList = new Playlist();
            tempPlayList.data = new List<Music>();
            int index = 0;
            foreach (var file in di.GetFiles())
            {
                try
                {
                    Music music = new Music();
                    string temp = file.Name.Substring(file.Name.Length - 4);
                    if (temp == "meta")
                        continue;

                    ////todo..주석 해체 테브릿
                    string tempPath = path + "/" + file.Name;
                    TagLib.File tempFile = TagLib.File.Create(tempPath);
                    string title = tempFile.Tag.Title;
                    string[] artist = tempFile.Tag.AlbumArtists;
                    TimeSpan duration = tempFile.Properties.Duration;

                    music.index = index;
                    music.title = title;
                    music.artist = artist[0];
                    music.path = tempPath;
                    music.duration = duration.TotalSeconds;
                    tempPlayList.data.Add(music);// = music;
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
                index++;
            }
            playlist = tempPlayList;
        }
        else
        {
            Debug.Log("Not Found Folder");
            playlist = null;
        }
    }

}
