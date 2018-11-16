#!/usr/bin/env python
import numpy as np
import math as m
from numpy.linalg import inv
from numpy import linalg as la
from time import time


class ur5:
    def __init__(self):
        self.a = [0, -0.425, -0.39225, 0, 0, 0]
        self.d = [0.089159, 0, 0, 0.10915, 0.09465, 0.0823]
        self.alpha = [m.pi / 2, 0, 0, m.pi / 2, -m.pi / 2, 0]

    def DH(self, a, alpha, d, theta):

        dh = np.array([
            [m.cos(theta), -m.sin(theta) * m.cos(alpha), m.sin(theta) * m.sin(alpha), a * m.cos(theta)],
            [m.sin(theta), m.cos(theta) * m.cos(alpha), -m.cos(theta) * m.sin(alpha), a * m.sin(theta)],
            [0, m.sin(alpha), m.cos(alpha), d],
            [0, 0, 0, 1]
        ])
        for i in range(np.shape(dh)[0]):
            for j in range(np.shape(dh)[1]):
                if abs(dh[i, j]) < 0.0001:
                    dh[i, j] = 0.0

        return dh

    def fwd_kin(self, joints):
        T01 = self.DH(self.a[0], self.alpha[0], self.d[0], joints[0])
        T12 = self.DH(self.a[1], self.alpha[1], self.d[1], joints[1])
        T23 = self.DH(self.a[2], self.alpha[2], self.d[2], joints[2])
        T34 = self.DH(self.a[3], self.alpha[3], self.d[3], joints[3])
        T45 = self.DH(self.a[4], self.alpha[4], self.d[4], joints[4])
        T56 = self.DH(self.a[5], self.alpha[5], self.d[5], joints[5])

        return np.dot(np.dot(np.dot(np.dot(np.dot(T01, T12), T23), T34), T45), T56)

    def inv_kin(self, pose):
        # pose is the 4x4 matrix of the end effector
        # DH parameters
        theta = np.zeros((6, 8))

        # theta1
        temp1 = np.array([0, 0, -self.d[5], 1])
        temp1.shape = (4, 1)
        temp2 = np.array([0, 0, 0, 1])
        temp2.shape = (4, 1)
        p05 = np.dot(pose, temp1) - temp2
        psi = m.atan2(p05[1], p05[0])
        if self.d[3] / m.sqrt(p05[1]**2 + p05[0]**2) > 1:
            phi = 0
        else:
            phi = m.acos(self.d[3] / m.sqrt(p05[1]**2 + p05[0]**2))
        theta[0, :4] = m.pi / 2 + psi + phi
        theta[0, 4:8] = m.pi / 2 + psi - phi

        # theta5
        for c in [0, 4]:
            T10 = inv(self.DH(self.a[0], self.alpha[0], self.d[0], theta[0, c]))
            T16 = np.dot(T10, pose)
            p16z = T16[2, 3]
            if (p16z - self.d[3]) / self.d[5] > 1:
                t5 = 0
            else:
                t5 = m.acos((p16z - self.d[3]) / self.d[5])
            theta[4, c:c + 1 + 1] = t5
            theta[4, c + 2:c + 3 + 1] = -t5

        # theta6
        for c in [0, 2, 4, 6]:
            T01 = self.DH(self.a[0], self.alpha[0], self.d[0], theta[0, c])
            T61 = np.dot(inv(pose), T01)
            T61zy = T61[1, 2]
            T61zx = T61[0, 2]
            t5 = theta[4, c]
            theta[5, c:c + 1 + 1] = m.atan2(-T61zy / m.sin(t5), T61zx / m.sin(t5))

        # theta3
        for c in [0, 2, 4, 6]:
            T10 = inv(self.DH(self.a[0], self.alpha[0], self. d[0], theta[0, c]))
            T65 = inv(self.DH(self.a[5], self.alpha[5], self.d[5], theta[5, c]))
            T54 = inv(self.DH(self.a[4], self.alpha[4], self.d[4], theta[4, c]))
            T14 = np.dot(np.dot(T10, pose), np.dot(T65, T54))
            temp1 = np.array([0, -self.d[3], 0, 1])
            temp1.shape = (4, 1)
            temp2 = np.array([0, 0, 0, 1])
            temp2.shape = (4, 1)
            p13 = np.dot(T14, temp1) - temp2
            p13norm2 = la.norm(p13)**2
            if (p13norm2 - self.a[1]**2 - self.a[2]**2) / (2 * self.a[1] * self.a[2]) > 1:
                t3p = 0
            else:
                t3p = m.acos((p13norm2 - self.a[1]**2 - self.a[2]**2) / (2 * self.a[1] * self.a[2]))
            theta[2, c] = t3p
            theta[2, c + 1] = -t3p

            # theta2 theta4
        for c in range(8):
            T10 = inv(self.DH(self.a[0], self.alpha[0], self.d[0], theta[0, c]))
            T65 = inv(self.DH(self.a[5], self.alpha[5], self.d[5], theta[5, c]))
            T54 = inv(self.DH(self.a[4], self.alpha[4], self.d[4], theta[4, c]))
            T14 = np.dot(np.dot(T10, pose), np.dot(T65, T54))
            temp1 = np.array([0, -self.d[3], 0, 1])
            temp1.shape = (4, 1)
            temp2 = np.array([0, 0, 0, 1])
            temp2.shape = (4, 1)
            p13 = np.dot(T14, temp1) - temp2
            p13norm = la.norm(p13)
            theta[1, c] = -m.atan2(p13[1], -p13[0]) + m.asin(self.a[2] * m.sin(theta[2, c]) / p13norm)
            T32 = inv(self.DH(self.a[2], self.alpha[2], self.d[2], theta[2, c]))
            T21 = inv(self.DH(self.a[1], self.alpha[1], self.d[1], theta[1, c]))
            T34 = np.dot(np.dot(T32, T21), T14)
            theta[3, c] = m.atan2(T34[1, 0], T34[0, 0])

        for i in range(np.shape(theta)[0]):
            for j in range(np.shape(theta)[1]):
                if theta[i, j] > m.pi:
                    theta[i, j] = theta[i, j] - 2 * m.pi
                if theta[i, j] < -m.pi:
                    theta[i, j] = theta[i, j] + 2 * m.pi

        return theta