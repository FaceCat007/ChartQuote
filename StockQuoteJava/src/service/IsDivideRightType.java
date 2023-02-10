/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package service;

/**
 *
 * 复权状态
 */
public enum IsDivideRightType {
    /*
     * 不复权
     */
     Non,
    /*
     * 前复权
     */
     Forward,
    /*
     * 后复权
     */
     Backward;

    public int getValue() {
        return this.ordinal();
    }

    public static IsDivideRightType forValue(int value) {
        return values()[value];
    }
}
