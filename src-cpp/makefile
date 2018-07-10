CXXFLAGS=-Wall -std=c++17 -march=native -O2
BINDIR=bin
SRCDIR=randomjs
OBJDIR=obj
EXECUTABLE=$(BINDIR)/randomjs
LDFLAGS=-Wl,-Bstatic -lboost_system -lboost_filesystem -Wl,-Bdynamic -lpthread -lgcc
SOURCES=$(wildcard $(SRCDIR)/*.c*)
OBJS=$(patsubst %.cpp,$(OBJDIR)/%.o,$(notdir $(SOURCES))) $(OBJDIR)/blake2b.o

$(EXECUTABLE): $(OBJS) | $(BINDIR)
	$(CXX) $(OBJS) $(LDFLAGS) -o $@ 

$(OBJDIR)/blake2b.o: $(SRCDIR)/blake2/blake2b.c | $(OBJDIR)
	$(CXX) $(CXXFLAGS) -c $< -o $@

$(OBJDIR)/%.o: $(SRCDIR)/%.cpp | $(OBJDIR)
	$(CXX) $(CXXFLAGS) -c $< -o $@
  
$(OBJDIR):
	mkdir $(OBJDIR)
  
$(BINDIR):
	mkdir $(BINDIR)

clean:
	rm -f $(EXECUTABLE) $(OBJDIR)/*.o
