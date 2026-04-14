<script lang="ts">
  import { Users, BellRing, Activity, Globe } from "@lucide/svelte";
</script>

<div class="h-full bg-[#3A506B] w-full flex flex-col items-center justify-center px-6 font-['Inter'] text-center overflow-hidden">
  
  <h1 class="text-[#EFF5FF] text-6xl md:text-[80px] font-['Righteous'] leading-tight mb-5 drop-shadow-[0_4px_4px_rgba(0,0,0,0.25)]">
    Welcome To<br/>
    Queue<span class="text-[#5BC0BE]">Smart</span>
  </h1>
  
  <p class="text-[18px] md:text-[20px] text-white/90 mb-10 max-w-[700px] leading-relaxed">
    An intelligent queue management platform for service providers and customers that enhances the waiting experience in real time.
  </p>

  <div class="flex flex-wrap justify-center gap-6 md:gap-14 border border-white/20 bg-white/5 rounded-[16px] p-6 md:p-8 w-full max-w-[900px] shadow-lg backdrop-blur-sm">
    
    <div class="flex flex-col items-center gap-3 w-[130px]">
       <div class="p-3 bg-[#1C2541]/50 rounded-full shadow-inner border border-white/10">
         <Users class="w-6 h-6 text-[#5BC0BE]" />
       </div>
       <span class="text-[14px] md:text-[15px] text-white leading-snug font-medium">Join queues remotely</span>
    </div>

    <div class="flex flex-col items-center gap-3 w-[130px]">
       <div class="p-3 bg-[#1C2541]/50 rounded-full shadow-inner border border-white/10">
         <BellRing class="w-6 h-6 text-[#5BC0BE]" />
       </div>
       <span class="text-[14px] md:text-[15px] text-white leading-snug font-medium">Recieve instant alerts</span>
    </div>

    <div class="flex flex-col items-center gap-3 w-[130px]">
       <div class="p-3 bg-[#1C2541]/50 rounded-full shadow-inner border border-white/10">
         <Activity class="w-6 h-6 text-[#5BC0BE]" />
       </div>
       <span class="text-[14px] md:text-[15px] text-white leading-snug font-medium">Live progress tracking</span>
    </div>

    <div class="flex flex-col items-center gap-3 w-[130px]">
       <div class="p-3 bg-[#1C2541]/50 rounded-full shadow-inner border border-white/10">
         <Globe class="w-6 h-6 text-[#5BC0BE]" />
       </div>
       <span class="text-[14px] md:text-[15px] text-white leading-snug font-medium">Monitor and manage services</span>
    </div>

  </div>

</div>