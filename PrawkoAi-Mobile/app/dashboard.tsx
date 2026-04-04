import { MaterialCommunityIcons, MaterialIcons } from "@expo/vector-icons";

import { useRouter } from "expo-router";

import React, { useContext } from "react";

import {
  ScrollView,
  StatusBar,
  Text,
  TouchableOpacity,
  View,
} from "react-native";

import { useSafeAreaInsets } from "react-native-safe-area-context";

import { AuthContext } from "./_layout";
import i18n from "./utils/translations";

export default function DashboardScreen() {
  const insets = useSafeAreaInsets();

  const { signOut } = useContext(AuthContext);

  const router = useRouter();

  const startExam = () => {
    router.push("/exam/examRules");
  };

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <StatusBar barStyle="default" />

      <View
        style={{ paddingTop: insets.top }}
        className="px-4 pb-4 flex-row items-center justify-between bg-[#f6f6f8]/80 dark:bg-[#111621]/80 border-b border-[#1544b2]/10"
      >
        <View className="flex-row items-center gap-3">
          <View className="w-10 h-10 rounded-full bg-white dark:bg-slate-800 items-center justify-center border border-[#1544b2]/10 shadow-sm">
            <MaterialIcons name="person" size={24} color="#1544b2" />
          </View>
          <View>
            <Text className="text-lg font-bold text-slate-900 dark:text-white leading-none">
              PrawkoAi
            </Text>
            <Text className="text-[10px] text-[#1544b2] font-bold uppercase tracking-wider">
              {i18n.t("premium_account")}
            </Text>
          </View>
        </View>

        <TouchableOpacity className="p-2 rounded-full">
          <MaterialIcons name="notifications-none" size={26} color="#1544b2" />
        </TouchableOpacity>
      </View>

      <ScrollView
        className="flex-1"
        showsVerticalScrollIndicator={false}
        contentContainerStyle={{ paddingBottom: 140 }}
      >
        <View className="p-4">
          <View className="bg-white dark:bg-slate-800 rounded-xl p-5 border border-[#1544b2]/5 shadow-sm">
            <View className="flex-row items-center justify-between mb-4">
              <Text className="text-sm text-slate-500 dark:text-slate-400 font-medium">
                {i18n.t("learning_progress")}
              </Text>
              <Text className="text-[#1544b2] font-bold text-xl">65%</Text>
            </View>
            <View className="w-full bg-slate-100 dark:bg-slate-700 rounded-full h-1.5 mb-3 overflow-hidden">
              <View className="bg-[#1544b2] h-full" style={{ width: "65%" }} />
            </View>
            <View className="flex-row justify-between items-center">
              <Text className="text-[11px] text-slate-400 font-medium">
                130 z 200 {i18n.t("questions_mastered")}
              </Text>
              <Text className="text-[11px] text-green-600 font-bold">
                +12 {i18n.t("today")}
              </Text>
            </View>
          </View>
        </View>

        <View className="px-4 py-2">
          <View className="relative overflow-hidden rounded-xl bg-[#1544b2] p-6 shadow-md">
            <View className="flex-row items-center gap-2 mb-2">
              <MaterialCommunityIcons name="brain" size={16} color="white" />
              <Text className="text-[10px] font-bold tracking-widest uppercase text-white/80">
                {i18n.t("ai_suggestion")}
              </Text>
            </View>
            <Text className="text-xl font-bold text-white mb-2">
              {i18n.t("daily_plan")}
            </Text>
            <Text className="text-white/80 text-sm mb-5 leading-5">
              {i18n.t("ai_recommendation_text")}
            </Text>
            <TouchableOpacity className="bg-white px-5 py-2 rounded-lg self-start active:opacity-90">
              <Text className="text-[#1544b2] font-bold text-xs uppercase">
                {i18n.t("train_now")}
              </Text>
            </TouchableOpacity>
          </View>
        </View>

        <View className="p-4">
          <Text className="text-slate-800 dark:text-slate-200 font-bold mb-3 px-1">
            {i18n.t("main_menu")}
          </Text>
          <TouchableOpacity className="bg-white dark:bg-slate-800 p-4 rounded-xl border border-[#1544b2]/5 shadow-sm mb-3 flex-row items-center justify-between">
            <View className="flex-row items-center gap-4">
              <View className="w-12 h-12 rounded-lg bg-blue-50 dark:bg-blue-900/20 items-center justify-center">
                <MaterialIcons name="local-library" size={26} color="#1544b2" />
              </View>
              <View>
                <Text className="font-bold text-slate-900 dark:text-white text-base">
                  {i18n.t("start_learning")}
                </Text>
                <Text className="text-xs text-slate-500">
                  {i18n.t("continue_course")}
                </Text>
              </View>
            </View>
            <MaterialIcons name="chevron-right" size={20} color="#cbd5e1" />
          </TouchableOpacity>

          <TouchableOpacity
            onPress={startExam}
            className="bg-white dark:bg-slate-800 p-4 rounded-xl border border-[#1544b2]/5 shadow-sm mb-3 flex-row items-center justify-between"
          >
            <View className="flex-row items-center gap-4">
              <View className="w-12 h-12 rounded-lg bg-green-50 dark:bg-green-900/20 items-center justify-center">
                <MaterialIcons
                  name="assignment-turned-in"
                  size={26}
                  color="#ea580c"
                />
              </View>
              <View>
                <Text className="font-bold text-slate-900 dark:text-white text-base">
                  {i18n.t("exam_simulation")}
                </Text>
                <Text className="text-xs text-slate-500">
                  {i18n.t("official_database")}
                </Text>
              </View>
            </View>
            <MaterialIcons name="chevron-right" size={20} color="#cbd5e1" />
          </TouchableOpacity>
        </View>

        <View className="px-4 py-2 flex-row gap-4">
          <View className="flex-1 bg-white dark:bg-slate-800 p-4 rounded-xl border border-[#1544b2]/5 shadow-sm">
            <Text className="text-[10px] font-bold text-slate-400 mb-2 uppercase">
              {i18n.t("streak")}
            </Text>
            <View className="flex-row items-center gap-2">
              <MaterialCommunityIcons name="fire" size={20} color="#f97316" />
              <Text className="text-lg font-bold text-slate-900 dark:text-white">
                5 {i18n.t("days")}
              </Text>
            </View>
          </View>
          <View className="flex-1 bg-white dark:bg-slate-800 p-4 rounded-xl border border-[#1544b2]/5 shadow-sm">
            <Text className="text-[10px] font-bold text-slate-400 mb-2 uppercase">
              {i18n.t("avg_score")}
            </Text>
            <View className="flex-row items-center gap-2">
              <MaterialIcons name="trending-up" size={20} color="#1544b2" />
              <Text className="text-lg font-bold text-slate-900 dark:text-white">
                84%
              </Text>
            </View>
          </View>
        </View>

        <TouchableOpacity
          onPress={signOut}
          className="mt-6 mx-4 p-4 rounded-xl border border-red-100 dark:border-red-900/20 items-center"
        >
          <Text className="text-red-600 font-bold">{i18n.t("logout")}</Text>
        </TouchableOpacity>
      </ScrollView>

      <View className="absolute bottom-0 left-0 right-0 bg-white/95 dark:bg-slate-900/95 border-t border-slate-100 dark:border-slate-800 px-6 pb-10 pt-3 flex-row justify-between items-center">
        <TouchableOpacity className="items-center">
          <MaterialIcons name="home" size={24} color="#1544b2" />
          <Text className="text-[10px] font-bold text-[#1544b2] mt-1">
            {i18n.t("nav_home")}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity className="items-center">
          <MaterialIcons name="leaderboard" size={24} color="#94a3b8" />
          <Text className="text-[10px] font-bold text-slate-400 mt-1">
            {i18n.t("nav_stats")}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity className="items-center">
          <MaterialCommunityIcons name="controller" size={24} color="#94a3b8" />
          <Text className="text-[10px] font-bold text-slate-400 mt-1">
            {i18n.t("nav_learn")}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity className="items-center">
          <MaterialIcons name="school" size={24} color="#94a3b8" />
          <Text className="text-[10px] font-bold text-slate-400 mt-1">
            {i18n.t("nav_school")}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity
          className="items-center"
          onPress={() => router.push("/exam/examHistory")}
        >
          <MaterialIcons name="account-circle" size={24} color="#94a3b8" />
          <Text className="text-[10px] font-bold text-slate-400 mt-1">
            {i18n.t("nav_profile")}
          </Text>
        </TouchableOpacity>
      </View>
    </View>
  );
}
