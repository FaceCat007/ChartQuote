package main;
import service.*;
import java.util.*;


/*
* 启动类
*/
public class Quote {
    public static void main(String[] args) {
        DataCenter.startService();
        String str = "0";
        if (str.equals("1"))
        {
            DataCenter.m_historyService.fallSecurityDatas();
        }
        else if (str.equals("2"))
        {
            DataCenter.m_historyService.fallMinuteSecurityDatas();
        }
    }
}
