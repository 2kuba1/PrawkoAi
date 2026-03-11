import { MaterialIcons } from "@expo/vector-icons";
import { Stack, useRouter } from "expo-router";
import React from "react";
import {
  ScrollView,
  StatusBar,
  Text,
  TouchableOpacity,
  useColorScheme,
  View,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";

export default function ExamRulesScreen() {
  const insets = useSafeAreaInsets();
  const router = useRouter();

  const colorScheme = useColorScheme();
  const iconColor = colorScheme === "dark" ? "#ffffff" : "#0f172a";

  const startExam = () => {
    router.push("/examSimulation");
  };

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <Stack.Screen options={{ headerShown: false }} />
      <StatusBar barStyle="light-content" />

      {/* Header */}
      <View
        style={{ paddingTop: insets.top }}
        className="flex-row items-center justify-between px-4 py-3 bg-white dark:bg-slate-900 border-b border-slate-200 dark:border-slate-800"
      >
        <TouchableOpacity
          onPress={() => router.back()}
          className="p-2 rounded-full active:bg-slate-200 dark:active:bg-slate-800"
        >
          <MaterialIcons name="arrow-back" size={24} color={iconColor} />
        </TouchableOpacity>
        <Text className="text-[#0f172a] dark:text-white text-lg font-bold tracking-tight flex-1 text-center pr-10">
          Zasady Egzaminu
        </Text>
      </View>

      <ScrollView
        className="flex-1"
        contentContainerStyle={{ paddingHorizontal: 16, paddingBottom: 120 }}
      >
        {/* General information */}
        <Text className="text-[#0f172a] dark:text-white text-2xl font-bold mt-6 mb-4">
          Ogólne informacje
        </Text>

        <View className="space-y-4">
          <View className="flex-row items-center bg-white dark:bg-slate-900 rounded-2xl p-4 shadow-sm border border-slate-200 dark:border-slate-800 mb-3">
            <View className="w-12 h-12 items-center justify-center rounded-lg bg-[#1544b2]/10 mr-4">
              <MaterialIcons name="quiz" size={28} color="#1544b2" />
            </View>
            <View className="flex-1">
              <Text className="text-[#0f172a] dark:text-white text-base font-bold">
                Liczba pytań: 32
              </Text>
              <Text className="text-slate-500 dark:text-slate-400 text-xs">
                Całkowita liczba pytań w teście
              </Text>
            </View>
          </View>

          <View className="flex-row items-center bg-white dark:bg-slate-900 rounded-2xl p-4 shadow-sm border border-slate-200 dark:border-slate-800">
            <View className="w-12 h-12 items-center justify-center rounded-lg bg-[#1544b2]/10 mr-4">
              <MaterialIcons name="timer" size={28} color="#1544b2" />
            </View>
            <View className="flex-1">
              <Text className="text-[#0f172a] dark:text-white text-base font-bold">
                Czas egzaminu: maks. 25 min
              </Text>
              <Text className="text-slate-500 dark:text-slate-400 text-xs">
                Całkowity czas na rozwiązanie
              </Text>
            </View>
          </View>
        </View>

        {/* Question structure */}
        <Text className="text-[#0f172a] dark:text-white text-2xl font-bold mt-8 mb-4">
          Struktura pytań
        </Text>

        <View className="bg-white dark:bg-slate-900 rounded-2xl p-4 shadow-sm border border-slate-200 dark:border-slate-800 mb-3 flex-row justify-between items-start">
          <View>
            <Text className="text-[#1544b2] font-bold text-xs tracking-widest uppercase">
              Pytania podstawowe
            </Text>
            <Text className="text-[#0f172a] dark:text-white text-lg font-bold mt-1">
              20 pytań
            </Text>
          </View>
          <View className="bg-[#1544b2]/5 px-2 py-1 rounded">
            <Text className="text-[#1544b2] text-[10px] font-semibold">
              35 sek / pytanie
            </Text>
          </View>
        </View>

        <View className="bg-white dark:bg-slate-900 rounded-2xl p-4 shadow-sm border border-slate-200 dark:border-slate-800 mb-3 flex-row justify-between items-start">
          <View>
            <Text className="text-[#1544b2] font-bold text-xs tracking-widest uppercase">
              Pytania specjalistyczne
            </Text>
            <Text className="text-[#0f172a] dark:text-white text-lg font-bold mt-1">
              12 pytań
            </Text>
          </View>
          <View className="bg-[#1544b2]/5 px-2 py-1 rounded">
            <Text className="text-[#1544b2] text-[10px] font-semibold">
              50 sek / pytanie
            </Text>
          </View>
        </View>

        {/* Points */}
        <Text className="text-[#0f172a] dark:text-white text-2xl font-bold mt-8 mb-4">
          Punktacja
        </Text>

        <View className="bg-[#1544b2] rounded-3xl p-6 shadow-xl overflow-hidden relative">
          <View className="flex-row justify-between items-center border-b border-white/20 pb-3 mb-3">
            <Text className="text-white/80 font-medium">Każde pytanie</Text>
            <Text className="text-white text-xl font-bold">1, 2 lub 3 pkt</Text>
          </View>
          <View className="flex-row justify-between items-center border-b border-white/20 pb-3 mb-3">
            <Text className="text-white/80 font-medium">Suma punktów</Text>
            <Text className="text-white text-xl font-bold">74 pkt</Text>
          </View>
          <View className="flex-row justify-between items-center bg-white/10 p-3 rounded-lg">
            <Text className="text-white font-bold">Warunek zdania</Text>
            <Text className="text-white text-2xl font-black italic">
              min. 68 pkt
            </Text>
          </View>

          <View className="absolute -right-4 -bottom-4 opacity-10">
            <MaterialIcons name="military-tech" size={100} color="white" />
          </View>
        </View>

        {/* Info Box */}
        <View className="mt-8 flex-row items-center space-x-3 p-4 bg-yellow-50 dark:bg-yellow-900/10 border border-yellow-200 dark:border-yellow-900/30 rounded-xl">
          <MaterialIcons name="info" size={20} color="#ca8a04" />
          <Text className="flex-1 text-[11px] text-yellow-800 dark:text-yellow-500 font-medium leading-4">
            Pamiętaj, że po zatwierdzeniu odpowiedzi nie możesz do niej wrócić.
            Skup się i uważnie czytaj każde pytanie.
          </Text>
        </View>
      </ScrollView>

      {/* Sticky Footer */}
      <View
        style={{ paddingBottom: insets.bottom + 16 }}
        className="absolute bottom-0 left-0 right-0 p-4 bg-white/95 dark:bg-slate-900/95 border-t border-slate-100 dark:border-slate-800"
      >
        <TouchableOpacity
          onPress={startExam}
          activeOpacity={0.8}
          className="w-full bg-[#1544b2] h-16 rounded-2xl flex-row items-center justify-center shadow-lg shadow-[#1544b2]/30"
        >
          <Text className="text-white font-bold text-lg mr-2">
            Rozpocznij Egzamin
          </Text>
          <MaterialIcons name="play-arrow" size={24} color="white" />
        </TouchableOpacity>
      </View>
    </View>
  );
}
