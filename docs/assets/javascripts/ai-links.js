(function () {
  const docsUrl = "https://dependable-intelligent-systems-lab.github.io/EDDI-Editor/";
  const repoUrl = "https://github.com/Dependable-Intelligent-Systems-Lab/EDDI-Editor";
  const prompt = [
    "Use the EDDI Editor documentation and source repository to help me.",
    "Docs: " + docsUrl,
    "Repository: " + repoUrl,
    "Focus on ODE, HiP-HOPS, Dymodia, DDI import, model conversion, and the WPF editor workflow."
  ].join("\n");

  const encodedPrompt = encodeURIComponent(prompt);
  const providers = [
    {
      label: "Open in ChatGPT",
      mark: "AI",
      href: "https://chatgpt.com/?q=" + encodedPrompt
    },
    {
      label: "Open in Claude",
      mark: "A",
      href: "https://claude.ai/new?q=" + encodedPrompt
    },
    {
      label: "Open in Gemini",
      mark: "G",
      href: "https://aistudio.google.com/app/prompts/new?text=" + encodedPrompt
    },
    {
      label: "Open in Kimi",
      mark: "K",
      href: "https://www.kimi.com/chat?q=" + encodedPrompt
    }
  ];

  function providerLink(provider) {
    const anchor = document.createElement("a");
    anchor.className = "ai-links-panel__item";
    anchor.href = provider.href;
    anchor.target = "_blank";
    anchor.rel = "noopener";
    anchor.innerHTML = [
      '<span class="ai-links-panel__mark">' + provider.mark + "</span>",
      "<span>" + provider.label + "</span>",
      '<span class="ai-links-panel__external" aria-hidden="true">↗</span>'
    ].join("");
    return anchor;
  }

  function mountPanel() {
    if (document.querySelector("[data-ai-links-panel]")) {
      return;
    }

    const scrollWrap = document.querySelector(".md-sidebar--primary .md-sidebar__scrollwrap");
    if (!scrollWrap) {
      return;
    }

    const panel = document.createElement("section");
    panel.className = "ai-links-panel";
    panel.setAttribute("data-ai-links-panel", "true");

    const title = document.createElement("p");
    title.className = "ai-links-panel__title";
    title.textContent = "Explore with AI";

    const list = document.createElement("div");
    list.className = "ai-links-panel__list";
    providers.forEach(function (provider) {
      list.appendChild(providerLink(provider));
    });

    panel.appendChild(title);
    panel.appendChild(list);
    scrollWrap.appendChild(panel);
  }

  function mountFab() {
    if (document.querySelector("[data-ai-fab]")) {
      return;
    }

    const fab = document.createElement("a");
    fab.className = "ai-fab";
    fab.href = providers[0].href;
    fab.target = "_blank";
    fab.rel = "noopener";
    fab.setAttribute("data-ai-fab", "true");
    fab.innerHTML = '<span>Ask AI</span><span class="ai-fab__dot" aria-hidden="true"></span>';
    document.body.appendChild(fab);
  }

  function mount() {
    mountPanel();
    mountFab();
  }

  if (typeof document$ !== "undefined") {
    document$.subscribe(mount);
  }

  document.addEventListener("DOMContentLoaded", mount);
})();
