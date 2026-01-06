export interface ScheduleParticipant {
  participantId: number;
  fullName: string;
}

export interface InstructorWeeklySchedule {
  lessonId: number;
  lessonTitle: string;
   date: string;   
  day: string;
  startTime: string;
  description?: string;
  endTime: string;
  participantCount: number;
  participants: ScheduleParticipant[];
}
