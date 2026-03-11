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
import { useSafeAreaInsets } from "react-native-safe-area-context";

export default function ExamRulesScreen() {
  const insets = useSafeAreaInsets();
  const router = useRouter();

  const startExam = () => {
    router.push("/examSimulation");
  };

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <Stack.Screen options={{ headerShown: false }} />
      <StatusBar barStyle="default" />

      {/* Header */}
      <View
        style={{ paddingTop: insets.top }}
        className="flex-row items-center p-4 bg-[#f6f6f8]/80 dark:bg-[#111621]/80 backdrop-blur-md border-b border-[#1544b2]/10"
      >
        <TouchableOpacity
          onPress={() => router.back()}
          className="p-2 hover:bg-[#1544b2]/10 rounded-full"
        >
          <MaterialIcons name="arrow-back" size={24} color="#1544b2" />
        </TouchableOpacity>
        <Text className="text-xl font-bold ml-2 text-slate-900 dark:text-white">
          Zasady Egzaminu
        </Text>
      </View>

      <ScrollView
        className="flex-1 px-4"
        showsVerticalScrollIndicator={false}
        contentContainerStyle={{ paddingTop: 20, paddingBottom: 150 }}
      >
        {/* General Information */}
        <View className="flex-row justify-between items-center mb-4">
          <Text className="text-lg font-bold text-slate-800 dark:text-slate-200">
            Ogólne informacje
          </Text>
        </View>

        <View className="space-y-3">
          <View className="flex-row items-center bg-white dark:bg-slate-800 p-4 rounded-xl border border-[#1544b2]/5 shadow-sm mb-3">
            <View className="w-12 h-12 rounded-lg bg-blue-50 dark:bg-blue-900/20 items-center justify-center">
              <MaterialIcons name="quiz" size={26} color="#1544b2" />
            </View>
            <View className="ml-4">
              <Text className="text-slate-900 dark:text-white font-bold text-base">
                Liczba pytań: 32
              </Text>
              <Text className="text-slate-500 dark:text-slate-400 text-xs">
                Pełna baza oficjalna
              </Text>
            </View>
          </View>

          <View className="flex-row items-center bg-white dark:bg-slate-800 p-4 rounded-xl border border-[#1544b2]/5 shadow-sm mb-3">
            <View className="w-12 h-12 rounded-lg bg-blue-50 dark:bg-blue-900/20 items-center justify-center">
              <MaterialIcons name="timer" size={26} color="#1544b2" />
            </View>
            <View className="ml-4">
              <Text className="text-slate-900 dark:text-white font-bold text-base">
                Czas trwania: 25 min
              </Text>
              <Text className="text-slate-500 dark:text-slate-400 text-xs">
                Odliczanie w czasie rzeczywistym
              </Text>
            </View>
          </View>
        </View>

        {/* Question Structure*/}
        <Text className="text-lg font-bold text-slate-800 dark:text-slate-200 mt-6 mb-4">
          Struktura testu
        </Text>

        <View className="bg-white dark:bg-slate-800 rounded-xl border border-[#1544b2]/5 shadow-sm overflow-hidden mb-3">
          <View className="p-4 border-b border-slate-50 dark:border-slate-700 flex-row justify-between items-center">
            <View>
              <Text className="text-[#1544b2] font-bold text-[10px] uppercase tracking-widest">
                Podstawowe
              </Text>
              <Text className="text-slate-900 dark:text-white font-bold text-lg">
                20 pytań
              </Text>
            </View>
            <View className="bg-slate-50 dark:bg-slate-700 px-3 py-1 rounded-full">
              <Text className="text-slate-500 dark:text-slate-400 text-[10px] font-bold">
                35s / pytanie
              </Text>
            </View>
          </View>
          <View className="p-4 flex-row justify-between items-center">
            <View>
              <Text className="text-[#1544b2] font-bold text-[10px] uppercase tracking-widest">
                Specjalistyczne
              </Text>
              <Text className="text-slate-900 dark:text-white font-bold text-lg">
                12 pytań
              </Text>
            </View>
            <View className="bg-slate-50 dark:bg-slate-700 px-3 py-1 rounded-full">
              <Text className="text-slate-500 dark:text-slate-400 text-[10px] font-bold">
                50s / pytanie
              </Text>
            </View>
          </View>
        </View>

        {/* Points */}
        <View className="mt-8 rounded-xl bg-[#1544b2] p-6 shadow-xl shadow-blue-900/20 relative overflow-hidden">
          <View className="absolute -right-6 -bottom-6 opacity-10">
            <MaterialCommunityIcons name="seal" size={120} color="white" />
          </View>

          <Text className="text-white/70 text-[10px] font-bold uppercase tracking-[2px] mb-4">
            System Oceniania
          </Text>

          <View className="flex-row justify-between items-end mb-4">
            <View>
              <Text className="text-white/80 text-xs">Suma punktów</Text>
              <Text className="text-white text-3xl font-bold">74 pkt</Text>
            </View>
            <View className="items-end">
              <Text className="text-white/80 text-xs">Próg zaliczenia</Text>
              <Text className="text-white text-3xl font-bold">68 pkt</Text>
            </View>
          </View>

          <View className="h-[1px] bg-white/20 w-full mb-4" />

          <View className="flex-row items-center">
            <MaterialIcons name="stars" size={20} color="#fbbf24" />
            <Text className="text-white/90 text-sm ml-2 font-medium">
              Pytania punktowane za 1, 2 lub 3 pkt
            </Text>
          </View>
        </View>

        {/* Info Box */}
        <View className="mt-6 flex-row items-start p-4 bg-orange-50 dark:bg-orange-900/10 border border-orange-100 dark:border-orange-900/20 rounded-xl">
          <MaterialIcons name="info" size={20} color="#ea580c" />
          <Text className="flex-1 text-[12px] text-orange-800 dark:text-orange-400 ml-3 leading-5">
            Uwaga: Po zatwierdzeniu odpowiedzi nie ma możliwości powrotu do
            pytania. Czytaj uważnie treść i analizuj materiały wideo.
          </Text>
        </View>
      </ScrollView>

      {/* Sticky Footer */}
      <View
        style={{ paddingBottom: insets.bottom + 16 }}
        className="absolute bottom-0 left-0 right-0 p-4 bg-[#f6f6f8]/95 dark:bg-[#111621]/95 border-t border-[#1544b2]/10"
      >
        <TouchableOpacity
          onPress={startExam}
          activeOpacity={0.9}
          className="w-full bg-[#1544b2] h-14 rounded-xl flex-row items-center justify-center shadow-lg shadow-blue-900/30"
        >
          <Text className="text-white font-bold text-base mr-2">
            Zatwierdzam i zaczynam
          </Text>
          <MaterialIcons name="chevron-right" size={24} color="white" />
        </TouchableOpacity>
      </View>
    </View>
  );
}
