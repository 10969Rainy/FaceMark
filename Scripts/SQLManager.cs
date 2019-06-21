using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SQLManager : MonoBehaviour {

    //数据库连接对象
    private SqliteConnection connection;
    //数据库命令
    private SqliteCommand command;
    //数据读取定义
    private SqliteDataReader reader;

    //本地数据库名字
    public string sqlName;

    public void Start()
    {
        CreateSQL();
        OpenSQLAndConnect();
    }

    //创建数据库文件
    public void CreateSQL()
    {
        if (!File.Exists(Application.streamingAssetsPath + "/" + sqlName))
        {
            connection = new SqliteConnection("data source=" + Application.streamingAssetsPath + "/" + sqlName);
            connection.Open();
            CreateSQLTable(
                "number",
                "CREATE TABLE number(" +
                "ID INT ," +
                "Name TEXT ," +
                "Money INT ," +
                "Adress TEXT)",
                null, null);
            connection.Close();
            return;
        }
    }

    //打开数据库
    public void OpenSQLAndConnect()
    {
        connection=new SqliteConnection("data source=" + Application.streamingAssetsPath + "/" + sqlName);
        connection.Open();
    }

    //执行SQL命令，并返回一个SqliteDataReader对象
    public SqliteDataReader ExecteSQLCommand(string queryString)
    {
        command = connection.CreateCommand();
        command.CommandText = queryString;
        reader = command.ExecuteReader();
        return reader;
    }

    //通过调用SQL语句，在数据库中创建一个表，顶定义表中的行的名字和对应的数据类型
    public SqliteDataReader CreateSQLTable(string tableName, string commandStr = null, string[] columnNames = null, string[] dataTypes = null)
    {
        return ExecteSQLCommand(commandStr);
    }

    //关闭数据库连接，每次测试结束的时候都调用关闭数据库连接
    public void CloseSQLConnection()
    {
        if (command != null)
        {
            command.Cancel();
        }

        if (reader != null)
        {
            reader.Close();
        }

        if (connection != null)
        {
            connection.Close();
        }

        command = null;
        reader = null;
        connection = null;

        print("已经断开数据库连接");
    }

    private void OnApplicationQuit()
    {
        CloseSQLConnection();
        print("程序退出");
    }
}
