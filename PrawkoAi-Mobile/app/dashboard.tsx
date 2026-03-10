import { MaterialCommunityIcons, MaterialIcons } from "@expo/vector-icons";
import { Stack, useRouter } from "expo-router";
import React, { useContext } from "react";
import {
  Image,
  ScrollView,
  StatusBar,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import {
  SafeAreaProvider,
  useSafeAreaInsets,
} from "react-native-safe-area-context";
import { AuthContext } from "./_layout";

export default function LoginScreen() {
  return (
    <SafeAreaProvider>
      <Stack.Screen options={{ headerShown: false }} />
      <DashboardContent />
    </SafeAreaProvider>
  );
}

function DashboardContent() {
  const insets = useSafeAreaInsets();
  const { signOut, token } = useContext(AuthContext);
  const router = useRouter();

  const startExam = () => {
    router.push("/examRulesScreen");
  };

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <StatusBar
        barStyle="dark-content"
        backgroundColor="#111621"
        translucent
      />

      {/* Header */}
      <View
        style={{ paddingTop: insets.top }}
        className="px-4 py-4 flex-row items-center justify-between border-b border-[#1544b2]/10 bg-white dark:bg-slate-900"
      >
        <View className="flex-row items-center gap-3">
          <View className="w-10 h-10 rounded-full bg-[#1544b2]/10 items-center justify-center overflow-hidden border border-[#1544b2]/20">
            <Image
              source={{
                uri: "",
              }}
              className="w-full h-full"
            />
          </View>
          <View>
            <Text className="text-lg font-bold text-slate-900 dark:text-white leading-none">
              PrawkoAi
            </Text>
            <Text className="text-[10px] text-[#1544b2] font-bold uppercase tracking-wider">
              Premium
            </Text>
          </View>
        </View>
        <TouchableOpacity className="w-10 h-10 rounded-full items-center justify-center bg-white dark:bg-slate-800 shadow-sm border border-slate-100">
          <MaterialIcons name="notifications-none" size={24} color="#64748b" />
        </TouchableOpacity>
      </View>

      <ScrollView
        className="flex-1"
        contentContainerStyle={{ paddingBottom: 120 }}
      >
        {/* Progress Section */}
        <View className="p-4">
          <View className="bg-white dark:bg-slate-900 rounded-2xl p-5 shadow-sm border border-[#1544b2]/5">
            <View className="flex-row items-center justify-between mb-4">
              <Text className="font-bold text-slate-800 dark:text-slate-100">
                Twój progres
              </Text>
              <Text className="text-[#1544b2] font-black text-xl">65%</Text>
            </View>
            <View className="w-full bg-[#1544b2]/10 rounded-full h-3 mb-3 overflow-hidden">
              <View
                className="bg-[#1544b2] h-full rounded-full"
                style={{ width: "65%" }}
              />
            </View>
            <View className="flex-row justify-between items-center">
              <Text className="text-xs text-slate-500 font-medium">
                130/200 przerobionych pytań
              </Text>
              <View className="px-2 py-1 bg-green-100 dark:bg-green-900/30 rounded-full">
                <Text className="text-[10px] font-bold text-green-700 dark:text-green-400">
                  +12 dzisiaj
                </Text>
              </View>
            </View>
          </View>
        </View>
        <View className="flex-1 items-center justify-center bg-white dark:bg-slate-900 p-6">
          <Text className="text-2xl font-bold mb-4 text-slate-900 dark:text-white">
            Witaj w Dashboardzie!
          </Text>
          <TouchableOpacity
            onPress={signOut}
            className="bg-red-500 w-full h-14 items-center justify-center rounded-xl"
          >
            <Text className="text-white font-bold">Wyloguj (API + Local)</Text>
          </TouchableOpacity>
        </View>
        {/* AI Recommendation Card */}
        <View className="px-4 py-2">
          <View className="relative overflow-hidden rounded-3xl bg-[#1544b2] p-6 shadow-xl shadow-[#1544b2]/20">
            <View className="absolute -top-10 -right-10 w-32 h-32 bg-white/10 rounded-full blur-3xl" />

            <View className="flex-row items-center gap-2 mb-3">
              <MaterialCommunityIcons name="brain" size={18} color="white" />
              <Text className="text-[10px] font-black tracking-[2px] uppercase text-white/90">
                Rekomendacja AI
              </Text>
            </View>
            <Text className="text-2xl font-bold text-white mb-2">
              Dzienny plan AI
            </Text>
            <Text className="text-white/80 text-[15px] leading-5 mb-5">
              Patrząc na twoje wyniki powinieneś skupić się na
              <Text className="font-bold text-white underline">
                {" "}
                'skrzyżowaniach'{" "}
              </Text>{" "}
            </Text>
            <TouchableOpacity className="bg-white self-start px-6 py-2.5 rounded-xl">
              <Text className="text-[#1544b2] font-bold text-sm uppercase tracking-tight">
                Powtórz teraz
              </Text>
            </TouchableOpacity>
          </View>
        </View>

        {/* Main Actions */}
        <View className="p-4">
          <Text className="text-slate-900 dark:text-white font-bold mb-4 px-1 text-lg">
            Menu główne
          </Text>

          <TouchableOpacity className="flex-row items-center gap-4 p-4 bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-[#1544b2]/5 mb-4 active:opacity-70">
            <View className="w-14 h-14 rounded-2xl bg-[#1544b2]/10 items-center justify-center">
              <MaterialIcons name="local-library" size={30} color="#1544b2" />
            </View>
            <View className="flex-1">
              <Text className="font-bold text-slate-900 dark:text-white text-lg">
                Zacznij naukę
              </Text>
              <Text className="text-sm text-slate-500">
                Kontynuuj gdzie skończyłeś
              </Text>
            </View>
            <MaterialIcons name="chevron-right" size={24} color="#cbd5e1" />
          </TouchableOpacity>

          <TouchableOpacity
            onPress={startExam}
            className="flex-row items-center gap-4 p-4 bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-[#1544b2]/5 active:opacity-70"
          >
            <View className="w-14 h-14 rounded-2xl bg-orange-100 dark:bg-orange-900/20 items-center justify-center">
              <MaterialIcons
                name="assignment-turned-in"
                size={30}
                color="#ea580c"
              />
            </View>
            <View className="flex-1">
              <Text className="font-bold text-slate-900 dark:text-white text-lg">
                Symulacja egzaminu
              </Text>
              <Text className="text-sm text-slate-500">
                Pełny egzamin pod presją czasu
              </Text>
            </View>
            <MaterialIcons name="chevron-right" size={24} color="#cbd5e1" />
          </TouchableOpacity>
        </View>

        {/* Quick Stats */}
        <View className="px-4 py-2 flex-row gap-4">
          <View className="flex-1 bg-white dark:bg-slate-900 p-4 rounded-2xl shadow-sm border border-[#1544b2]/5">
            <Text className="text-[10px] font-bold text-slate-400 mb-2 uppercase">
              Aktualny Streak
            </Text>
            <View className="flex-row items-center gap-2">
              <MaterialCommunityIcons name="fire" size={22} color="#f97316" />
              <Text className="text-xl font-black dark:text-white">5 Dni</Text>
            </View>
          </View>
          <View className="flex-1 bg-white dark:bg-slate-900 p-4 rounded-2xl shadow-sm border border-[#1544b2]/5">
            <Text className="text-[10px] font-bold text-slate-400 mb-2 uppercase">
              Średni wynik egzaminu
            </Text>
            <View className="flex-row items-center gap-2">
              <MaterialIcons name="verified" size={22} color="#1544b2" />
              <Text className="text-xl font-black dark:text-white">84%</Text>
            </View>
          </View>
        </View>
      </ScrollView>

      {/* Bottom Navigation */}
      <View className="absolute bottom-0 left-0 right-0 bg-white/95 dark:bg-slate-900/95 border-t border-slate-100 dark:border-slate-800 px-6 pb-10 pt-3 flex-row justify-between items-center">
        <TouchableOpacity className="items-center">
          <MaterialIcons name="home" size={24} color="#1544b2" />
          <Text className="text-[10px] font-bold text-[#1544b2] mt-1">
            Home
          </Text>
        </TouchableOpacity>

        <TouchableOpacity className="items-center">
          <MaterialIcons name="leaderboard" size={24} color="#94a3b8" />
          <Text className="text-[10px] font-bold text-slate-400 mt-1">
            Statystyki
          </Text>
        </TouchableOpacity>

        <TouchableOpacity className="items-center">
          <MaterialCommunityIcons name="controller" size={24} color="#94a3b8" />
          <Text className="text-[10px] font-bold text-slate-400 mt-1">
            Nauka
          </Text>
        </TouchableOpacity>

        <TouchableOpacity className="items-center">
          <MaterialIcons name="school" size={24} color="#94a3b8" />
          <Text className="text-[10px] font-bold text-slate-400 mt-1">
            Szkoła Jazdy
          </Text>
        </TouchableOpacity>

        <TouchableOpacity className="items-center">
          <MaterialIcons name="account-circle" size={24} color="#94a3b8" />
          <Text className="text-[10px] font-bold text-slate-400 mt-1">
            Profil
          </Text>
        </TouchableOpacity>
      </View>
    </View>
  );
}
