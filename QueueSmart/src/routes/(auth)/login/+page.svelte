<script lang="ts">
  import { Eye, EyeOff } from "@lucide/svelte";

  let email: string = '';
  let password: string = '';
  let errorMessage: string = '';
  let showPassword: boolean = false;

  $: isEmailValid = email.includes('@') && email.includes('.');
  // password must be atleast 8 characters and include a special character
  $: isPasswordValid = password.length >= 8 && /[!@#$%^&*(),.?":{}|<>]/.test(password);
  
  $: isFormValid = isEmailValid && isPasswordValid;

  function handleLogin(): void {
    if (!isFormValid) {
      errorMessage = 'Provide a valid email and password';
      return;
    }
    errorMessage = '';
    alert(`wow you logged in`);
  }

  function togglePassword() {
    showPassword = !showPassword;
  }
</script>

<div class="min-h-screen bg-[#3A506B] flex flex-col items-center justify-center p-6 font-['Inter']">
  
  <h1 class="text-[#EFF5FF] text-5xl md:text-[72px] mt-4 mb-8 text-center tracking-wide font-['Righteous'] drop-shadow-[0_4px_4px_rgba(0,0,0,0.25)]">
    Queue<span class="text-[#5BC0BE]">Smart</span>
  </h1>

  <div class="relative w-full max-w-[450px] mb-8">
    
    <div class="absolute inset-0 bg-[#1C2541] rounded-[8px] translate-x-6 -translate-y-4"></div>
    <div class="absolute inset-0 bg-[#5BC0BE] rounded-[8px] -translate-x-4 translate-y-4"></div>
    
    <div class="relative bg-[#FFFFFF] rounded-[8px] py-8 px-6 md:px-10 flex flex-col items-center shadow-xl">
      
      <h2 class="text-[#3A506B] text-[32px] md:text-[36px] font-bold mb-6 tracking-wide">
        LOGIN
      </h2>

      {#if errorMessage}
        <div class="w-full bg-red-100 border border-red-400 text-red-700 p-2 rounded mb-4 text-sm text-center">
          {errorMessage}
        </div>
      {/if}

      <form on:submit|preventDefault={handleLogin} class="w-full flex flex-col gap-6">
        
        <div class="relative w-full">
          <label for="email" class="absolute -top-3 left-4 bg-[#FFFFFF] px-1 text-[12px] text-[#6C6A6A]">
            Email
          </label>
          <input 
            type="email" 
            id="email"
            bind:value={email}
            class="w-full h-[44px] border border-[#6C6A6A] rounded-[8px] px-4 text-[#3A506B] placeholder:text-[#AFADAD] focus:outline-none focus:border-[#5BC0BE] focus:ring-1 focus:ring-[#5BC0BE] bg-transparent transition-colors"
            required
          />
        </div>

        <div class="relative w-full">
          <label for="password" class="absolute -top-3 left-4 bg-[#FFFFFF] px-1 text-[12px] text-[#6C6A6A]">
            Password
          </label>
          <input 
            type={showPassword ? "text" : "password"} 
            id="password"
            bind:value={password}
            class="w-full h-[44px] border border-[#6C6A6A] rounded-[8px] px-4 pr-10 text-[#3A506B] placeholder:text-[#AFADAD] focus:outline-none focus:border-[#5BC0BE] focus:ring-1 focus:ring-[#5BC0BE] bg-transparent transition-colors"
            required
          />
          <button type="button" on:click={togglePassword} class="absolute inset-y-0 right-0 pr-3 flex items-center text-[#6C6A6A] hover:text-[#5BC0BE] transition-colors">
            {#if showPassword}
              <EyeOff class="w-5 h-5" />
            {:else}
              <Eye class="w-5 h-5" />
            {/if}
          </button>
        </div>

        <button 
          type="submit"
          class="mt-2 w-[181px] h-[43px] bg-[#3A506B] hover:bg-[#2c3d52] text-[#FFFFFF] text-[20px] rounded-[8px] mx-auto transition-colors"
        >
          Login
        </button>

      </form>

      <div class="mt-8 text-[#6C6A6A] text-[14px] text-center">
        Not a member? <a href="/register" class="text-[#3A506B] hover:underline ml-1">Register Now</a>
      </div>

    </div>
  </div>
</div>