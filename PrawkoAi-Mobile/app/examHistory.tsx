import { MaterialCommunityIcons, MaterialIcons } from "@expo/vector-icons";
import { Stack, useRouter } from "expo-router";
import React from "react";
import {
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

export default function ExamHistoryScreen() {
  return (
    <SafeAreaProvider>
      <Stack.Screen options={{ headerShown: false }} />
      <ExamHistory />
    </SafeAreaProvider>
  );
}

function ExamHistory() {
  const insets = useSafeAreaInsets();
  const router = useRouter();

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <StatusBar barStyle="dark-content" />

      {/* Header */}
      <View
        style={{ paddingTop: insets.top }}
        className="flex-row items-center p-4 bg-[#f6f6f8]/80 dark:bg-[#111621]/80 border-b border-blue-900/10"
      >
        <TouchableOpacity
          className="p-2 rounded-full"
          onPress={() => router.back()}
        >
          <MaterialCommunityIcons name="arrow-left" size={24} color="#1544b2" />
        </TouchableOpacity>
        <Text className="text-xl font-bold ml-2 flex-1 text-slate-900 dark:text-white">
          Historia Egzaminów
        </Text>
        <TouchableOpacity className="p-2">
          <MaterialCommunityIcons
            name="filter-variant"
            size={24}
            color="#1544b2"
          />
        </TouchableOpacity>
      </View>

      <ScrollView className="flex-1" showsVerticalScrollIndicator={false}>
        {/* Stats Overview */}
        <View className="p-4 flex-row gap-4">
          <View className="flex-1 bg-white dark:bg-slate-800 p-4 rounded-xl border border-blue-900/10 shadow-sm">
            <Text className="text-sm text-slate-500 dark:text-slate-400 font-medium">
              Średni wynik
            </Text>
            <View className="flex-row items-baseline mt-1">
              <Text className="text-2xl font-bold text-[#1544b2]">68</Text>
              <Text className="text-sm text-slate-400 ml-1">/74 pkt</Text>
            </View>
            <View className="mt-2 h-1 w-full bg-slate-100 dark:bg-slate-700 rounded-full overflow-hidden">
              <View className="bg-[#1544b2] h-full w-[92%]" />
            </View>
          </View>

          <View className="flex-1 bg-white dark:bg-slate-800 p-4 rounded-xl border border-blue-900/10 shadow-sm">
            <Text className="text-sm text-slate-500 dark:text-slate-400 font-medium">
              Ukończone testy
            </Text>
            <Text className="text-2xl font-bold text-[#1544b2] mt-1">14</Text>
            <View className="mt-2 flex-row items-center">
              <MaterialCommunityIcons
                name="trending-up"
                size={12}
                color="#16a34a"
              />
              <Text className="text-[10px] text-green-600 font-medium ml-1">
                +2 w tym tygodniu
              </Text>
            </View>
          </View>
        </View>

        {/* Section Title */}
        <View className="px-4 py-2 flex-row justify-between items-center">
          <Text className="text-lg font-bold text-slate-800 dark:text-slate-200">
            Ostatnie podejścia
          </Text>
          <Text className="text-[10px] text-slate-500 font-bold uppercase tracking-wider">
            Maj 2024
          </Text>
        </View>

        {/* Exam List */}
        <View className="px-4 space-y-3 pb-24">
          {/* Exam Item - Passed */}
          <ExamItem
            date="12.05.2024"
            score="72/74"
            status="Zaliczony"
            passed={true}
          />
          <ExamItem
            date="10.05.2024"
            score="69/74"
            status="Zaliczony"
            passed={true}
          />

          {/* Exam Item - Failed */}
          <ExamItem
            date="08.05.2024"
            score="58/74"
            status="Niezaliczony"
            passed={false}
          />

          <View className="py-2 mt-4">
            <Text className="text-[10px] text-slate-500 font-bold uppercase tracking-wider">
              Kwiecień 2024
            </Text>
          </View>

          <ExamItem
            date="28.04.2024"
            score="74/74"
            status="Zaliczony"
            passed={true}
          />
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

const ExamItem = ({ date, score, status, passed }: any) => (
  <TouchableOpacity className="bg-white dark:bg-slate-800 p-4 rounded-xl border border-blue-900/5 shadow-sm mb-3">
    <View className="flex-row justify-between items-start">
      <View className="flex-row items-center">
        <View
          className={`w-12 h-12 rounded-lg items-center justify-center ${passed ? "bg-green-50 dark:bg-green-900/20" : "bg-red-50 dark:bg-red-900/20"}`}
        >
          <MaterialIcons
            name={passed ? "assignment-turned-in" : "assignment-late"}
            size={24}
            color={passed ? "#16a34a" : "#dc2626"}
          />
        </View>
        <View className="ml-3">
          <Text className="font-bold text-slate-900 dark:text-white">
            {date}
          </Text>
          <Text className="text-sm text-slate-500">
            Wynik:{" "}
            <Text className="font-semibold text-slate-700 dark:text-slate-300">
              {score} pkt
            </Text>
          </Text>
        </View>
      </View>
      <View
        className={`${passed ? "bg-green-100 dark:bg-green-900/30" : "bg-red-100 dark:bg-red-900/30"} px-3 py-1 rounded-full`}
      >
        <Text
          className={`text-[10px] font-bold uppercase ${passed ? "text-green-700 dark:text-green-400" : "text-red-700 dark:text-red-400"}`}
        >
          {status}
        </Text>
      </View>
    </View>
  </TouchableOpacity>
);
