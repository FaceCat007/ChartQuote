/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package service;

/**
 * 复权类型
 */
public enum DivideRightType {
    ZengFa,
    PeiGu,
    PaiXi,
    GengMing,
    SongGu,
    ZhuanZeng,
    BingGu,
    ChaiGu,
    Jili;

    public int getValue() {
        return this.ordinal();
    }

    public static DivideRightType forValue(int value) {
        return values()[value];
    }
}
