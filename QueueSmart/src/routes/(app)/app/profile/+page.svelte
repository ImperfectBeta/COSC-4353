<script lang="ts">
    import { authSession } from "$lib/stores/auth";
    import { CircleUser } from "@lucide/svelte";
</script>

<div class="max-w-2xl mx-auto py-8">
    <div class="bg-white rounded-[16px] shadow-sm border border-gray-200 overflow-hidden">
        
        <div class="bg-[#1C2541] px-8 py-6 flex items-center gap-6">
            <div class="bg-[#5BC0BE] p-4 rounded-full">
                <CircleUser class="w-12 h-12 text-[#1C2541]" />
            </div>
            <div>
                <h1 class="text-2xl font-bold text-white capitalize">
                    {$authSession?.email?.split('@')[0] || "My Profile"}
                </h1>
                <p class="text-[#5BC0BE]">
                    {$authSession?.email || "No email found"}
                </p>
            </div>
        </div>

        <div class="p-8 space-y-6">
            <div>
                <h3 class="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-2">Account Role</h3>
                <p class="text-lg text-[#3A506B] capitalize">{$authSession?.role || "User"}</p>
            </div>
            
            <hr class="border-gray-100" />
            
            <div>
                <h3 class="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-2">Account Status</h3>
                <span class="inline-flex items-center px-4 py-1 rounded-full text-sm font-bold bg-[#5BC0BE]/20 text-[#1C2541]">
                    Active
                </span>
            </div>
        </div>
        
    </div>
</div>