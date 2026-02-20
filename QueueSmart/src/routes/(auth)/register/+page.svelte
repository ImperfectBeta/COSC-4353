<script lang="ts">
  import { Eye, EyeOff, ChevronDown } from "@lucide/svelte";

  let email: string = '';
  let role: string = 'user';
  let password: string = '';
  let confirmPassword: string = '';
  let errorMessage: string = '';
  
  let showPassword: boolean = false;

  $: isEmailValid = email.includes('@') && email.includes('.');
  // password must be atleast 8 characters and include a special character
  $: isPasswordValid = password.length >= 8 && /[!@#$%^&*(),.?":{}|<>]/.test(password);
  $: doPasswordsMatch = password === confirmPassword && password.length > 0;
  
  $: isFormValid = isEmailValid && isPasswordValid && doPasswordsMatch;

  function handleRegister(): void {
    if (!isEmailValid) {
      errorMessage = 'Please provide a valid email.';
      return;
    }
    if (!isPasswordValid) {
      errorMessage = 'Password must be at least 8 characters and include a special character.';
      return;
    }
    if (!doPasswordsMatch) {
      errorMessage = 'Passwords do not match.';
      return;
    }
    errorMessage = '';
    
    const roleName = role === 'admin' ? 'Administrator' : 'Basic User';
    alert(`you made an account, very cool`);
  }

  function togglePassword() {
    showPassword = !showPassword;
  }
</script>

<div class="min-h-screen bg-[#3A506B] flex flex-col items-center justify-center p-6 font-['Inter']">
  
  <h1 class="text-[#EFF5FF] text-5xl md:text-[72px] mt-4 mb-8 text-center tracking-wide font-['Righteous'] drop-shadow-[0_4px_4px_rgba(0,0,0,0.25)]">
    Queue<span class="text-[#5BC0BE]">Smart</span>
  </h1>

  <div class="relative w-full max-w-[450px] mb-4">
    
    <div class="absolute inset-0 bg-[#1C2541] rounded-[8px] translate-x-6 -translate-y-4"></div>
    <div class="absolute inset-0 bg-[#5BC0BE] rounded-[8px] -translate-x-4 translate-y-4"></div>
    
    <div class="relative bg-[#FFFFFF] rounded-[8px] py-6 px-6 md:px-10 flex flex-col items-center shadow-xl">
      
      <h2 class="text-[#3A506B] text-[28px] md:text-[32px] font-bold mb-4 tracking-wide text-center uppercase">
        Register
      </h2>

      {#if errorMessage}
        <div class="w-full bg-red-100 border border-red-400 text-red-700 p-2 rounded mb-4 text-sm text-center">
          {errorMessage}
        </div>
      {/if}

      <form on:submit|preventDefault={handleRegister} class="w-full flex flex-col gap-5">
        
        <div class="relative w-full">
          <label for="email" class="absolute -top-3 left-4 bg-[#FFFFFF] px-1 text-[12px] text-[#6C6A6A]">
            Email
          </label>
          <input 
            type="email" 
            id="email"
            bind:value={email}
            class="w-full h-[40px] border border-[#6C6A6A] rounded-[8px] px-4 text-[#3A506B] placeholder:text-[#AFADAD] focus:outline-none focus:border-[#5BC0BE] focus:ring-1 focus:ring-[#5BC0BE] bg-transparent transition-colors"
            required
          />
        </div>

        <div class="relative w-full">
          <label for="role" class="absolute -top-3 left-4 bg-[#FFFFFF] px-1 text-[12px] text-[#6C6A6A]">
            Account Type
          </label>
          <select 
            id="role"
            bind:value={role}
            class="w-full h-[40px] border border-[#6C6A6A] rounded-[8px] px-4 text-[#3A506B] focus:outline-none focus:border-[#5BC0BE] focus:ring-1 focus:ring-[#5BC0BE] bg-transparent transition-colors appearance-none cursor-pointer"
          >
            <option value="user">Basic User</option>
            <option value="admin">Administrator (Organization)</option>
          </select>
          <div class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-4 text-[#6C6A6A]">
            <ChevronDown class="h-4 w-4" />
          </div>
        </div>

        <div class="relative w-full">
          <label for="password" class="absolute -top-3 left-4 bg-[#FFFFFF] px-1 text-[12px] text-[#6C6A6A]">
            Password
          </label>
          <input 
            type={showPassword ? "text" : "password"} 
            id="password"
            bind:value={password}
            class="w-full h-[40px] border border-[#6C6A6A] rounded-[8px] px-4 pr-10 text-[#3A506B] placeholder:text-[#AFADAD] focus:outline-none focus:border-[#5BC0BE] focus:ring-1 focus:ring-[#5BC0BE] bg-transparent transition-colors"
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

        <div class="w-full flex flex-col">
          <div class="relative w-full">
            <label for="confirmPassword" class="absolute -top-3 left-4 bg-[#FFFFFF] px-1 text-[12px] text-[#6C6A6A] z-10">
              Confirm Password
            </label>
            <input 
              type={showPassword ? "text" : "password"} 
              id="confirmPassword"
              bind:value={confirmPassword}
              class="w-full h-[40px] border border-[#6C6A6A] rounded-[8px] px-4 pr-10 text-[#3A506B] placeholder:text-[#AFADAD] focus:outline-none focus:border-[#5BC0BE] focus:ring-1 focus:ring-[#5BC0BE] bg-transparent transition-colors relative z-0"
              required
            />
            <button type="button" on:click={togglePassword} class="absolute inset-y-0 right-0 pr-3 flex items-center text-[#6C6A6A] hover:text-[#5BC0BE] transition-colors z-10">
              {#if showPassword}
                <EyeOff class="w-5 h-5" />
              {:else}
                <Eye class="w-5 h-5" />
              {/if}
            </button>
          </div>
          
          <div class="w-full mt-1 flex flex-col gap-1 pl-1">
            <span class="text-[10px] text-[#6C6A6A]">* Password must be 8 characters and include a special character.</span>
            {#if confirmPassword.length > 0 && password !== confirmPassword}
              <span class="text-[11px] font-bold text-red-500">Passwords must match :/</span>
            {/if}
          </div>
        </div>

        <button 
          type="submit"
          class="mt-2 w-[181px] h-[40px] bg-[#3A506B] hover:bg-[#2c3d52] text-[#FFFFFF] text-[18px] rounded-[8px] mx-auto transition-colors"
        >
          Register
        </button>

      </form>

      <div class="mt-4 text-[#6C6A6A] text-[14px] text-center">
        Already a member? <a href="/login" class="text-[#3A506B] hover:underline ml-1">Login Here</a>
      </div>

    </div>
  </div>
</div>