<script lang="ts">
  import { page } from '$app/stores';

  const nav = [
    { href: '/admin',          label: 'Dashboard',          icon: '▦' },
    { href: '/admin/services', label: 'Service Management', icon: '⚙' },
    { href: '/admin/queue',    label: 'Queue Management',   icon: '≡' },
  ];
</script>

<div class="flex h-screen font-mono overflow-hidden" style="background:var(--color-foreground); color:var(--color-primary-foreground)">
  <!-- Sidebar -->
  <aside class="w-60 flex-shrink-0 flex flex-col" style="background:var(--color-muted-foreground); border-right:1px solid rgba(0,0,0,0.12)">
    <div class="px-6 py-6" style="border-bottom:1px solid rgba(0,0,0,0.12)">
      <span class="text-sm uppercase tracking-widest" style="color:var(--color-background)">QueueSmart</span>
      <h1 class="text-xl font-bold" style="color:var(--color-primary); margin-top:.25rem">Admin Panel</h1>
    </div>

    <nav class="flex-1 py-6 px-3" style="display:flex; flex-direction:column; gap:.25rem">
      {#each nav as item}
        {@const active = $page.url.pathname === item.href}
        <a
          href={item.href}
          class="flex items-center gap-3 px-4 py-2.5 rounded-lg text-sm"
          style={active
            ? 'background: rgba(91,192,190,0.08); color: var(--color-primary); border: 1px solid rgba(91,192,190,0.12)'
            : 'color: var(--color-background); border: 1px solid transparent'}
        >
          <span class="text-base">{item.icon}</span>
          {item.label}
        </a>
      {/each}
    </nav>

    <div class="px-6 py-5 text-xs" style="border-top:1px solid rgba(0,0,0,0.12); color:var(--color-background)">
      Logged in as <span style="color:var(--color-primary)">admin@queuesmart.io</span>
    </div>
  </aside>

  <!-- Main content -->
  <main class="flex-1 overflow-y-auto">
    <slot />
  </main>
</div>